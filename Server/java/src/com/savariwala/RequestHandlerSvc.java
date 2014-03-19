package com.savariwala;

import com.owlike.genson.Genson;
import com.savariwala.com.savariwala.js.*;
import com.vividsolutions.jts.geom.Coordinate;
import com.vividsolutions.jts.geom.Point;
import jdk.nashorn.internal.objects.Global;
import org.apache.http.client.fluent.Request;
import org.apache.thrift.TException;
import org.neo4j.gis.spatial.SimplePointLayer;
import org.neo4j.gis.spatial.SpatialDatabaseRecord;
import org.neo4j.gis.spatial.SpatialDatabaseService;
import org.neo4j.gis.spatial.pipes.GeoPipeFlow;
import org.neo4j.graphdb.*;
import org.neo4j.graphdb.factory.GraphDatabaseFactory;
import org.neo4j.tooling.GlobalGraphOperations;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.IOException;
import java.util.*;
import java.util.concurrent.CompletableFuture;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.atomic.AtomicLong;
import java.util.function.BinaryOperator;
import java.util.function.Function;
import java.util.stream.Collectors;

/**
 * Created with IntelliJ IDEA.
 * User: apoorvkul
 * Date: 8/3/14
 * Time: 3:48 PM
 * To change this template use File | Settings | File Templates.
 */

public class RequestHandlerSvc implements RequestHandler.Iface {
    private static final long WAITING_TIME_MIN_MSEC = 120*1000;    // Min 2 mins as buffer
    private static final long WAITING_TIME_MAX_MSEC = 600*1000;    // Max 10 mins wait
    private final String directionApiUrlFmt =
            "http://maps.googleapis.com/maps/api/directions/json?origin=%f,%f&destination=%f,%f&sensor=false&alternatives=true&departure_time=%d";

    final AtomicLong bookingIdCounter;

    private final Logger logger = LoggerFactory.getLogger(this.getClass().getName());
    final GraphDatabaseService graph;
    final SpatialDatabaseService db;
    final SimplePointLayer layer;
    final Label routeLbl;
    final Label bookingLbl;
    private final HashMap<Long, PooledRoute> pooledRoutes;
    final Genson genson = new Genson();


    public class BookingInfo {
        public CompletableFuture<String> fut;
    }

    HashMap<Long, BookingInfo> _pendingReq;

    public RequestHandlerSvc() {
        int instanceId = 1; // TODO should come from properties
        pooledRoutes = new HashMap<>();
        bookingIdCounter = new AtomicLong(new Date().getTime()/1000 + instanceId << 60);
        graph = new GraphDatabaseFactory().newEmbeddedDatabase("neo4j_db");
        Runtime.getRuntime().addShutdownHook(new Thread(() -> graph.shutdown()));
        try (Transaction tx = graph.beginTx())
        {
            db = new SpatialDatabaseService(graph);

            // TODO: DB might persist layers. Do I need to create layer on restart of program?
            routeLbl = DynamicLabel.label("Route");
            bookingLbl = DynamicLabel.label("Booking");
            if (db.containsLayer("LatLng"))
            {
                layer = (SimplePointLayer) db.getLayer("LatLng");
                reloadFromDB();
            }
            else layer = db.createSimplePointLayer("LatLng");
            tx.success();
        }
    }

    private void reloadFromDB() {
        for(Node routeNode : GlobalGraphOperations.at(graph).getAllNodesWithLabel(routeLbl))
            pooledRoutes.put (routeNode.getId(), new PooledRoute(routeNode));
    }

    class RouteMatchInfo
    {
        public int Count;
        public Long timeWaitingSec;
        public Node start;
        public Node end;

        RouteMatchInfo(int count, Node x, long diff)
        {
            Count = count;
            this.start = x;
            this.end = x;
            timeWaitingSec = diff;
        }
    }

    private SpatialDatabaseRecord upsert(double lat, double lng) {
        try
        {
            List<GeoPipeFlow> pipeFlows = layer.findClosestPointsTo(new Coordinate(lat, lng, 0.0));
            Optional<SpatialDatabaseRecord> recFound = pipeFlows.stream().map(x -> x.getRecord()).findFirst();
            if (recFound.isPresent())
            {
                // findClosestPointsTo returns nearest point rather than coinciding point in spite of search distance of 0.0
                // TODO will need to find better method to locate exact point
                Point pt = (Point)recFound.get().getGeometry();
                if(pt.getX() == lat && pt.getY() == lng) return recFound.get();
            }
            return layer.add(lat, lng);
        }
        catch (NullPointerException e)    // throws NullPointerException in case of empty graph
        {
            return layer.add(lat, lng);
        }
    }

    private Long createBooking(PooledRoute pooledRoute, BookingDetails booking)
    {
        Long bId =  bookingIdCounter.getAndIncrement();
        try (Transaction tx = graph.beginTx())
        {
            Node bookingNode = graph.createNode(bookingLbl);
            bookingNode.createRelationshipTo(pooledRoute.getRouteNode(), Utils.RelTypes.BOOKED);
            bookingNode.setProperty("BookingId", bId);
            bookingNode.setProperty("NumPax", booking.getNumPax());
            tx.success();
        }
        pooledRoute.addNumPax(graph, booking.getNumPax());
        return bId;
    }

    private Coordinate getCoordinate(Node node)
    {
        return ((Point) layer.getGeometryEncoder().decodeGeometry(node)).getCoordinate();
    }

    private void updateRouteMatch(HashMap<Long, RouteMatchInfo> matchedRoutes, Node routeNode, BookingDetails booking)
    {
        long diff = ((long)routeNode.getProperty("StartUtc")) - booking.getStartTime();
       // if (WAITING_TIME_MIN_MSEC <= diff && diff <= WAITING_TIME_MAX_MSEC)
        {
            RouteMatchInfo rmi = matchedRoutes.get(routeNode.getId());
            if(rmi == null) matchedRoutes.put(routeNode.getId(), new RouteMatchInfo(1, routeNode, diff));
            else
            {
                ++rmi.Count;
                rmi.end = routeNode;
            }
        }
    }

    @Override
    public long submitBooking(BookingDetails booking, String routeJson) throws ServerError, TException
    {
        try
        {
            if (routeJson == null || routeJson.isEmpty()) throw new ServerError("Empty/Null Route Provided", ErrorCode.InvalidArg);
            JsDirection jsDirection = genson.deserialize(routeJson, JsDirection.class);
            if (!jsDirection.getStatus().equals("OK"))
                throw new APIException("Directions API returned status: " + jsDirection.getStatus());
            Route topRoute = null;
            for (JsRoute route : jsDirection.getRoutes()) {
                JsCoordinate last = null;
                SpatialDatabaseRecord recStart = null;
                SpatialDatabaseRecord recEnd = null;
                MapPoint mapPointDst = null;
                HashMap<Long, RouteMatchInfo> matchedRoutes = new HashMap<>();
                final Route curRoute = new Route();
                if(topRoute == null) topRoute = curRoute; // Save best route to use in case we cant pool it
                curRoute.getStepInfos().add(new Route.StepInfo(0, 0));  // duration and distance to 0th node
                for (JsLeg leg : route.getLegs()) {
                    for (JsStep step : leg.getSteps()) {
                        JsCoordinate ptSrc = step.getStart_location();
                        JsCoordinate ptDst = step.getEnd_location();
                        curRoute.getStepInfos().add(new Route.StepInfo(step.getDuration().getValue(), step.getDistance().getValue()));
                        // TODO handle expiry of relationships and booking and
                        // deletion of nodes when it is no longer in way of any booking
                        try (Transaction tx = graph.beginTx()) {
                            if (last == null || (last.getLat() != ptSrc.getLat() && last.getLng() != ptSrc.getLng())) {
                                recStart = upsert(ptSrc.getLat(), ptSrc.getLng());
                            } else recStart = recEnd;
                            recEnd = upsert(ptDst.getLat(), ptDst.getLng());
                            recStart.getGeomNode().getRelationships(Utils.RelTypes.IN_WAY_OF).forEach(
                                    x -> updateRouteMatch(matchedRoutes, x.getEndNode(), booking));
                            curRoute.getPathNodes().add(recStart.getGeomNode());
                            last = ptSrc;
                            tx.success();
                        }
                    }
                }
                if(recEnd != null)
                {
                    try (Transaction tx = graph.beginTx()) {
                    recStart.getGeomNode().getRelationships(Utils.RelTypes.IN_WAY_OF).forEach(
                            x -> updateRouteMatch(matchedRoutes, x.getEndNode(), booking));
                        tx.success();
                    }
                    curRoute.getPathNodes().add(recEnd.getGeomNode());
                }

                List<Map.Entry<Long,RouteMatchInfo>> sortedEntries = getSortedMatches(matchedRoutes, curRoute.getPathNodes().size());
                for(int i = 0; i < sortedEntries.size(); ++i)
                {
                    Optional<Long> bId = poolRoutes(sortedEntries.get(i).getKey(), curRoute, booking);
                    if(bId.isPresent()) return bId.get();
                }
            }
            PooledRoute pr = new PooledRoute(topRoute, graph, routeLbl, booking);
            pooledRoutes.put(pr.getRouteNode().getId(), pr);
            return createBooking(pr, booking);
        }
        catch (InterruptedException e)
        {
            logger.error("Failed: ", e);
            throw new ServerError(e.getMessage(), ErrorCode.Interrupted);
        }
        catch (ServerError e)
        {
            logger.error("Failed: ", e);
            throw e;
        }
        catch(Exception e)
        {
            logger.error("Failed: ", e);
            throw new ServerError(e.toString(), ErrorCode.Unspecified);
        }
    }

    private List<Map.Entry<Long, RouteMatchInfo>> getSortedMatches(Map<Long, RouteMatchInfo> matchedRoutes, int size)
    {
        // TODO: score function to be experimented more
        Function<RouteMatchInfo, Integer> scoreFn = (rmi) ->
                (int) (100 * (2*rmi.Count/(double)size + rmi.timeWaitingSec/(double)WAITING_TIME_MAX_MSEC));
        return matchedRoutes.entrySet().stream()
                .sorted((x, y) -> scoreFn.apply(x.getValue()).compareTo(scoreFn.apply(y.getValue())))
                .collect(Collectors.toCollection(ArrayList::new));
    }

    private Optional<Long> poolRoutes(Long pooledRouteId, Route curRoute, BookingDetails booking) throws InterruptedException
    {
        PooledRoute pooledRoute = pooledRoutes.get(pooledRouteId);
        if(pooledRoute == null) return Optional.empty();

        ArrayList<CompletableFuture<Integer>>futures = new ArrayList<>();
        try (Transaction tx = graph.beginTx()) {
            final Coordinate curStart = getCoordinate(curRoute.getPathNodes().get(0));
            final Coordinate curEnd = getCoordinate(curRoute.getPathNodes().get(curRoute.getPathNodes().size() - 1));
            final Coordinate poolStart = getCoordinate(pooledRoute.getPathNodes().get(0));
            final Coordinate poolEnd = getCoordinate(pooledRoute.getPathNodes().get(pooledRoute.getPathNodes().size() - 1));

            if(curStart.equals(poolStart))
            {
                if(curEnd.equals(poolEnd))
                {
                    CompletableFuture<Integer> f = CompletableFuture.supplyAsync(() ->{
                        return fetchAndProcessRoutes(curStart, curEnd);
                    });
                    futures.add(f);
                }
                else
                {
                    futures.add(CompletableFuture.supplyAsync(() -> fetchAndProcessRoutes(curStart, curEnd, poolEnd)));
                    futures.add(CompletableFuture.supplyAsync(() -> fetchAndProcessRoutes(curStart, poolEnd, curEnd)));
                }
            }
            else
            {
                if(curEnd.equals(poolEnd))
                {
                    futures.add(CompletableFuture.supplyAsync(() -> fetchAndProcessRoutes(poolStart, curStart, curEnd)));
                    futures.add(CompletableFuture.supplyAsync(() -> fetchAndProcessRoutes(curStart, poolStart, curEnd)));
                }
                else
                {
                    futures.add(CompletableFuture.supplyAsync(() -> fetchAndProcessRoutes(poolStart, curStart, poolEnd, curEnd)));
                    futures.add(CompletableFuture.supplyAsync(() -> fetchAndProcessRoutes(curStart, poolStart, poolEnd, curEnd)));
                    futures.add(CompletableFuture.supplyAsync(() -> fetchAndProcessRoutes(poolStart, curStart, curEnd, poolEnd)));
                    futures.add(CompletableFuture.supplyAsync(() -> fetchAndProcessRoutes(curStart, poolStart, curEnd, poolEnd)));
                }
            }
            tx.success();
        }
        int min = Integer.MAX_VALUE;
        for (CompletableFuture<Integer> f : futures)
        {
            try
            {
                int res = f.get();
                if(res > 0) min = Integer.min(res, min);
            } catch (ExecutionException e) { logger.error("Failed: ", e); }
        }
        // Following checks should be on price also, once pricing model is in place
        double ratio = min / (double) Integer.max(pooledRoute.getDistance(), curRoute.getDistance());
        if (ratio <= 1.1 && isNumPaxOptimistic(pooledRoute.getNumPax() + booking.getNumPax()))
            return Optional.of(createBooking(pooledRoute, booking));
        else
            return Optional.empty();
    }

    // TODO:  It assuming 3 pax per car and also does not optimizes for group bookings which cant split across cars
    private boolean isNumPaxOptimistic(int numPax) {
        return true; //numPax % 3 == 1;
    }

    private int fetchAndProcessRoutes(Coordinate... wayPoints)
    {
        logger.info("In fetchAndProcessRoutes");
        long utc = new Date().getTime() / 1000;
        if(wayPoints.length < 2) throw new IllegalArgumentException("Unable to route with 0 way points");
        Coordinate dst = wayPoints[wayPoints.length - 1];
        String url = String.format(directionApiUrlFmt, wayPoints[0].x, wayPoints[0].y, dst.x, dst.y, utc);

        if(wayPoints.length > 2)
        {
            StringBuilder wpBuilder = new StringBuilder("&waypoints=optimize:true");
            for (int i = 1; i < wayPoints.length - 1; ++i)
            wpBuilder.append('|').append(wayPoints[i].x).append(',').append(wayPoints[i].y);
            url += wpBuilder.toString();
        }

        try
        {
            String jsonStr = Request.Get(url).execute().returnContent().asString();
            if (jsonStr == null || jsonStr.isEmpty()) throw new IOException("Unable to fetch Directions");
            JsDirection jsDirection = genson.deserialize(jsonStr, JsDirection.class);
            if (!jsDirection.getStatus().equals("OK"))
                throw new APIException("Directions API returned status: " + jsDirection.getStatus());
            if(jsDirection.getRoutes().isEmpty())
                throw new APIException("No Routes found by Directions API");
            BinaryOperator<Integer> adder = (a, b) -> a + b;
            // Sum up distances of all steps of all legs of first route
            return Utils.getValue(jsDirection.getRoutes().get(0).getLegs().stream()
                    .map(leg -> Utils.getValue(leg.getSteps().stream()
                            .map(x -> x.getDistance().getValue())
                            .reduce(adder), 0)
                    ).reduce(adder), 0);
        }
        catch (Exception e)
        {
            logger.error("Failed: ", e);
            return 0;
        }
    }

    @Override
    public BookingDetails getDetails(long bookingId) throws TException {
        return null;  //To change body of implemented methods use File | Settings | File Templates.
    }

    @Override
    public boolean cancel(long bookingId) throws TException {
        BookingInfo bookingInfo = _pendingReq.get(bookingId);
        if (bookingInfo == null) return false;   // TODO throw not found exception
        if (!bookingInfo.fut.isDone()) bookingInfo.fut.cancel(true);

        _pendingReq.remove(bookingId);
        return true;
    }

    public static void main(String [] args)
    {
        Logger logger = LoggerFactory.getLogger("Main");

        try {
            Properties prop = Utils.loadProperties(RequestHandlerSvc.class);
            Utils.runThriftServer(logger,
                    new RequestHandler.Processor(new RequestHandlerSvc()),
                    Integer.parseInt(prop.getProperty("port", "9091")));

        } catch (Exception ex) {
            logger.error("Failed: ", ex);
        }
    }
}


/* Dead Code:   Keeping exploratory dead code here till we finally feel confident about our approach

private Optional<Long> poolRoutes(Long pooledRouteId, RouteMatchInfo routeMatchInfo, Route curRoute)
    {
        PooledRoute pooledRoute = pooledRoutes.get(pooledRouteId);
        if(pooledRoute == null) return Optional.empty();

        int startPoolIdx = pooledRoute.getPathNodes().indexOf(routeMatchInfo.start);
        int startCurIdx = curRoute.getPathNodes().indexOf(routeMatchInfo.start);
        Coordinate startCommon = getCoordinate(routeMatchInfo.start);
        Coordinate startCur = getCoordinate(curRoute.getPathNodes().get(startCurIdx - 1));
        Coordinate startPool = getCoordinate(pooledRoute.getPathNodes().get(startPoolIdx - 1));

        //  Curr Route -- Common -- Pooled Route : This case cant be pooled
        //                  |
        if(new Envelope(startCur, startPool).contains(startCommon)) return Optional.empty();

        int endPoolIdx = pooledRoute.getPathNodes().indexOf(routeMatchInfo.end);
        int endCurIdx = curRoute.getPathNodes().indexOf(routeMatchInfo.end);
        Coordinate endCommon = getCoordinate(routeMatchInfo.end);
        Coordinate endCur = getCoordinate(curRoute.getPathNodes().get(endCurIdx + 1));
        Coordinate endPool = getCoordinate(pooledRoute.getPathNodes().get(endPoolIdx + 1));

        // Same check on end
        if(new Envelope(endCur, endPool).contains(endCommon)) return Optional.empty();

        List<Coordinate> wayPts = new ArrayList<>();
        Coordinate newStart;
        if (new Envelope(startPool, endCommon).contains(startCur))
        {
            newStart = startPool;
            //wayPts.add(GetCood curRoute.getPathNodes().get(startCurIdx - 1));
        }
        else
        {
            newStart = startCur;
        }

        Coordinate newEnd = getCoordinate(new Envelope(endPool, endCommon).contains(endCur) ?
                        pooledRoute.getPathNodes().get(endPoolIdx + 1): curRoute.getPathNodes().get(endCurIdx + 1));

        // TODO Validate from google and pool and return booking id


        fetchAndProcessRoutes(newStart, newEnd, wayPts) ;

        return Optional.empty();
    }

 */


