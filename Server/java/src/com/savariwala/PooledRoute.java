package com.savariwala;

import org.javatuples.Triplet;
import org.neo4j.graphdb.*;

import java.util.ArrayList;
import java.util.stream.Collector;
import java.util.stream.Stream;

/**
 * Created with IntelliJ IDEA.
 * User: apoorvkul
 * Date: 16/3/14
 * Time: 8:37 PM
 * To change this template use File | Settings | File Templates.
 */
public class PooledRoute extends Route
{
    private int numPax;

    public Node getRouteNode() {
        return routeNode;
    }

    private Node routeNode;

    public PooledRoute(Node routeNode)
    {
        this.routeNode = routeNode;
        final ArrayList<Triplet<Integer, Node, StepInfo>> entries = new ArrayList<>();
        routeNode.getRelationships(Utils.RelTypes.IN_WAY_OF, Direction.INCOMING).forEach(
                x -> entries.add(new Triplet<>(
                        (int) x.getProperty("StepOrder"),
                        x.getStartNode(),
                        new StepInfo((int) x.getProperty("EstArrivalTime"), (int) x.getProperty("StepDistance")))));
        entries.stream().sorted((x, y) -> x.getValue0().compareTo(y.getValue0())).forEachOrdered(x -> {
            pathNodes.add(x.getValue1());
            stepInfos.add(x.getValue2());
        });
        numPax = (int) routeNode.getProperty("NumPax");
    }

    public PooledRoute(Route route, GraphDatabaseService graph, Label routeLbl, BookingDetails bookingDetails)
    {
        super(route);
        try (Transaction tx = graph.beginTx()) {
            routeNode = graph.createNode(routeLbl);
            for(int i = 0; i < pathNodes.size(); ++i)
            {
                Relationship rel = pathNodes.get(i).createRelationshipTo(routeNode, Utils.RelTypes.IN_WAY_OF);
                rel.setProperty("StepOrder", i);
                rel.setProperty("EstArrivalTime", /*FIXME*/ stepInfos.get(i).estTimeArrival);
                rel.setProperty("StepDistance", stepInfos.get(i).distance);
            }
            routeNode.setProperty("Distance", getDistance());
            routeNode.setProperty("StartUtc", bookingDetails.getStartTime());
            routeNode.setProperty("NumPax", bookingDetails.getNumPax());
            routeNode.setProperty("StepCount", pathNodes.size());
            tx.success();
        }
    }

    public int getNumPax() {
        return numPax;
    }

    public void addNumPax(GraphDatabaseService graph, int delta) {
        try (Transaction tx = graph.beginTx()) {
            routeNode.setProperty("NumPax", numPax + delta);
            tx.success();
        }
        this.numPax += delta;
    }
}
