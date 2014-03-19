package com.savariwala.com.savariwala.js;

import java.util.List;

/**
 * Created with IntelliJ IDEA.
 * User: apoorvkul
 * Date: 18/3/14
 * Time: 10:43 AM
 * To change this template use File | Settings | File Templates.
 */

public class JsDirection {
    public String getStatus() {
        return status;
    }

    public void setStatus(String status) {
        this.status = status;
    }

    public List<JsRoute> getRoutes() {
        return routes;
    }

    public void setRoutes(List<JsRoute> routes) {
        this.routes = routes;
    }

    String status;
    List<JsRoute> routes;
}
