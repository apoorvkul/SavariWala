package com.savariwala;

import com.google.common.collect.Ordering;
import com.google.common.collect.TreeMultimap;
import org.apache.thrift.TException;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.sql.*;
import java.util.*;
import java.util.stream.Collector;
import java.util.stream.Collectors;

/**
 * Created with IntelliJ IDEA.
 * User: apoorvkul
 * Date: 5/3/14
 * Time: 8:39 PM
 * To change this template use File | Settings | File Templates.
 */
public class MapPointProviderSvc implements MapPointProvider.Iface {

    static final double kmPerDegreeLatitute =  111.2;
    class PointInfo {
        MapPoint mapPoint;
        boolean isSrc;
        boolean isDst;

        PointInfo(MapPoint mapPoint, boolean src, boolean dst) {
            this.mapPoint = mapPoint;
            isSrc = src;
            isDst = dst;
        }
    }
    List<PointInfo> pointInfos = new ArrayList<>();
    private  final Logger logger = LoggerFactory.getLogger(this.getClass().getName());

    private void loadRows(ResultSet res, List<PointInfo> pts)
    {
        try {
            while (res.next()) {
                MapPoint pt = new MapPoint();
                pt.latitude = res.getDouble("latitude");
                pt.longitude = res.getDouble("longitude");
                pt.description = res.getString("description");
                pts.add(new PointInfo(pt, res.getBoolean("is_src"), res.getBoolean("is_dst")));
            }
        } catch (SQLException e) {
            logger.error(e.toString());
        }
    }

    public MapPointProviderSvc() throws ClassNotFoundException, SQLException {
            Class.forName("org.postgresql.Driver");
            String url = "jdbc:postgresql:rideshare?user=rider&password=rider";
            final Connection conn = DriverManager.getConnection(url);
            Statement stmt = conn.createStatement();
            loadRows(stmt.executeQuery("SELECT latitude, longitude, description,is_src, is_dst FROM map_points"), pointInfos);
    }

    @Override
    // TODO grossly inefficient using unfriendly java containers :(
    // PointInfos should be in stored in Rtree rather
    public List<MapPoint> getMapPoint(boolean isSrc, double latitude, double longitude) throws TException {
        logger.info(String.format("Received (%f, %f)", latitude, longitude));
        TreeMultimap<Double,MapPoint> top5 = TreeMultimap.create(Ordering.natural().reverse(), Ordering.natural());
        double kmPerDegreeLongitude = kmPerDegreeLatitute * Math.abs(Math.cos(latitude));
        for(PointInfo ptInfo : pointInfos)
        {
            if (!(isSrc ? ptInfo.isSrc : ptInfo.isDst)) continue;
            MapPoint pt = ptInfo.mapPoint;
            double dist = kmPerDegreeLatitute *  Math.abs(latitude - pt.latitude)  + kmPerDegreeLongitude * Math.abs(longitude - pt.longitude);
            //double max = top5.isEmpty() ? Double.MAX_VALUE : top5.keySet().first();
           // if(dist < max)
           // {
                top5.put(dist, pt);
           // }
        }
        LinkedList<MapPoint> retVal = new LinkedList<>();
        top5.values().forEach(x -> retVal.addFirst(x)); // reverse
        return retVal.stream().limit(5).collect(Collectors.toList());
    }

    public static void main(String [] args) {
        Logger logger = LoggerFactory.getLogger("Main");

        try {
            Properties prop = Utils.loadProperties(MapPointProviderSvc.class);
            Utils.runThriftServer(logger,
                    new MapPointProvider.Processor(new MapPointProviderSvc()),
                    Integer.parseInt(prop.getProperty("port", "9091")));
        } catch (Exception x) {
            logger.error(x.toString());
        }
    }
}
