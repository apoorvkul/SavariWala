package com.savariwala.com.savariwala.js;

import java.util.List;

/**
 * Created with IntelliJ IDEA.
 * User: apoorvkul
 * Date: 18/3/14
 * Time: 10:46 AM
 * To change this template use File | Settings | File Templates.
 */
public class JsRoute {
    public List<JsLeg> getLegs() {
        return legs;
    }

    public void setLegs(List<JsLeg> legs) {
        this.legs = legs;
    }

    List<JsLeg> legs;
}
