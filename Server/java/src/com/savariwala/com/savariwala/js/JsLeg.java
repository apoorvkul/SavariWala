package com.savariwala.com.savariwala.js;

import java.util.List;

/**
 * Created with IntelliJ IDEA.
 * User: apoorvkul
 * Date: 18/3/14
 * Time: 10:49 AM
 * To change this template use File | Settings | File Templates.
 */
public class JsLeg {
    public List<JsStep> getSteps() {
        return steps;
    }

    public void setSteps(List<JsStep> steps) {
        this.steps = steps;
    }

    List<JsStep> steps;
    JsDisplayValue distance;

    public JsDisplayValue getDistance() {
        return distance;
    }

    public void setDistance(JsDisplayValue distance) {
        this.distance = distance;
    }
}
