package com.savariwala.com.savariwala.js;

/**
 * Created with IntelliJ IDEA.
 * User: apoorvkul
 * Date: 18/3/14
 * Time: 10:51 AM
 * To change this template use File | Settings | File Templates.
 */
public class JsStep {
    JsCoordinate start_location;
    JsCoordinate end_location;
    JsDisplayValue duration;
    JsDisplayValue distance;

    public JsDisplayValue getDistance() {
        return distance;
    }

    public void setDistance(JsDisplayValue distance) {
        this.distance = distance;
    }

    public JsCoordinate getStart_location() {
        return start_location;
    }

    public void setStart_location(JsCoordinate start_location) {
        this.start_location = start_location;
    }

    public JsCoordinate getEnd_location() {
        return end_location;
    }

    public void setEnd_location(JsCoordinate end_location) {
        this.end_location = end_location;
    }

    public JsDisplayValue getDuration() {
        return duration;
    }

    public void setDuration(JsDisplayValue duration) {
        this.duration = duration;
    }
}
