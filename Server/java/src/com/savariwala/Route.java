package com.savariwala;

import org.neo4j.graphdb.Node;

import java.util.ArrayList;

/**
 * Created with IntelliJ IDEA.
 * User: apoorvkul
 * Date: 16/3/14
 * Time: 9:47 PM
 * To change this template use File | Settings | File Templates.
 */
public class Route
{
    public Route()
    {
        pathNodes = new ArrayList<>();
        stepInfos = new ArrayList<>();
    }

    public Route(Route that) {
        this.pathNodes = that.pathNodes;
        this.stepInfos = that.stepInfos;
    }

    public ArrayList<Node> getPathNodes()
    {
        return pathNodes;
    }

    static public class StepInfo
    {
        int estTimeArrival;
        int distance;

        public StepInfo(int estTimeArrival, int distance) {
            this.estTimeArrival = estTimeArrival;
            this.distance = distance;
        }
    }
    protected final ArrayList<Node> pathNodes;

    public ArrayList<StepInfo> getStepInfos() {
        return stepInfos;
    }

    protected final ArrayList<StepInfo> stepInfos;

    public int getDistance(){
        return stepInfos.isEmpty() ? 0 : stepInfos.stream().map(x -> x.distance).reduce((a, b) -> a + b).get();
    }
}
