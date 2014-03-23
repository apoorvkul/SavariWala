package com.savariwala;

import com.vividsolutions.jts.geom.Coordinate;
import com.vividsolutions.jts.geom.Point;
import org.neo4j.gis.spatial.SimplePointLayer;
import org.neo4j.gis.spatial.SpatialDatabaseRecord;
import org.neo4j.gis.spatial.SpatialDatabaseService;
import org.neo4j.gis.spatial.pipes.GeoPipeFlow;
import org.neo4j.graphdb.*;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.sql.*;
import java.util.List;
import java.util.Optional;

/**
 * Created with IntelliJ IDEA.
 * User: apoorvkul
 * Date: 21/3/14
 * Time: 9:48 PM
 * To change this template use File | Settings | File Templates.
 */
public class DBSyncer {
    public void sync(GraphDatabaseService graph) {
        try {
            Class.forName("org.postgresql.Driver");
            String url = "jdbc:postgresql:rideshare?user=rider&password=rider";
            try (final Connection conn = DriverManager.getConnection(url)) {
                Statement stmt = conn.createStatement();
                txfrPoints(stmt.executeQuery(
                        "SELECT latitude, longitude, name, address, locality is_src, is_dst FROM map_points"),
                        graph);
            } catch (SQLException e) {
                e.printStackTrace();  //To change body of catch statement use File | Settings | File Templates.
            }
        } catch (ClassNotFoundException e) {
            e.printStackTrace();  //To change body of catch statement use File | Settings | File Templates.
        }
    }

    private  final Logger logger = LoggerFactory.getLogger(this.getClass().getName());

    private SpatialDatabaseRecord upsert(SimplePointLayer layer, double lat, double lng) {
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

    private void txfrPoints(ResultSet res, GraphDatabaseService graph)
    {
        try (Transaction tx = graph.beginTx())
        {
            SpatialDatabaseService db = new SpatialDatabaseService(graph);
            SimplePointLayer layer = (SimplePointLayer) db.getLayer("LatLng");
            Label terminal = DynamicLabel.label("Terminal");

            try {
                while (res.next()) {
                    SpatialDatabaseRecord rec = upsert(layer, res.getDouble("latitude"), res.getDouble("longitude"));
                    Node node = rec.getGeomNode();
                    node.addLabel(terminal);
                    node.setProperty("Name", res.getString("name"));
                    node.setProperty("Address", res.getString("address"));
                    node.setProperty("Locality", res.getString("locality"));
                    node.setProperty("IsSrc", res.getBoolean("is_src"));
                    node.setProperty("IsDst", res.getBoolean("is_dst"));
                    tx.success();

                }
            } catch (SQLException e) {
                logger.error(e.toString());
            }
        }
    }


}
