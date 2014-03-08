package com.savariwala;

import com.google.common.collect.Ordering;
import com.google.common.collect.SortedSetMultimap;
import com.google.common.collect.TreeMultimap;
import org.apache.thrift.TException;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.sql.*;
import java.util.ArrayList;
import java.util.List;
import java.util.Properties;

/**
 * Created with IntelliJ IDEA.
 * User: apoorvkul
 * Date: 5/3/14
 * Time: 8:39 PM
 * To change this template use File | Settings | File Templates.
 */
public class MapPointProviderSvc implements MapPointProvider.Iface {

    static final double kmPerDegreeLatitute =  111.2;
    List<MapPoint> _mapPoints = new ArrayList<>();
    List<MapPoint> _mapPointsSrc = new ArrayList<>();
    private  final Logger _logger = LoggerFactory.getLogger(this.getClass().getName());

    private void loadRows(ResultSet res, List<MapPoint> pts)
    {
        try {
            while (res.next()) {
                MapPoint pt = new MapPoint();
                pt.latitude = res.getDouble("latitude");
                pt.longitude = res.getDouble("Longitute");
                pt.description = res.getString("description");
                pts.add(pt);
            }
        } catch (SQLException e) {
            _logger.error(e.toString());
        }
    }

    public MapPointProviderSvc() throws ClassNotFoundException, SQLException {
            Class.forName("org.postgresql.Driver");
            String url = "jdbc:postgresql:rideshare?user=rider&password=rider";
            final Connection conn = DriverManager.getConnection(url);
            Statement stmt = conn.createStatement();
            loadRows(stmt.executeQuery("SELECT latitude, longitude, description FROM map_points"), _mapPoints);
            loadRows(stmt.executeQuery("SELECT latitude, longitude, description FROM src_map_points"), _mapPointsSrc);
    }

    @Override
    public List<MapPoint> getMapPoint(boolean isSrc, double latitude, double longitude) throws TException {
        TreeMultimap<Double,MapPoint> top10 = TreeMultimap.create(Ordering.natural().reverse(), Ordering.natural());

        double kmPerDegreeLongitude = kmPerDegreeLatitute * Math.cos(latitude);
        List<MapPoint> pts = isSrc ? _mapPointsSrc : _mapPoints;
        for(MapPoint pt: pts)
        {
            double dist = kmPerDegreeLatitute *  Math.abs(latitude - pt.latitude)  + kmPerDegreeLongitude * Math.abs(longitude - pt.longitude);
            double max = top10.keySet().first();
            if(dist < max)
            {
                top10.put(dist, pt);
                top10.remove(max, top10.get(max));
            }
        }
        ArrayList<MapPoint> retVal = new ArrayList<>();
        retVal.addAll(top10.values());
        return retVal;
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
