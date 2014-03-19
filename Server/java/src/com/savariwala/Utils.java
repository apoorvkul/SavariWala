package com.savariwala;

import org.apache.http.client.fluent.Request;
import org.apache.thrift.TProcessor;
import org.apache.thrift.server.TServer;
import org.apache.thrift.server.TSimpleServer;
import org.apache.thrift.transport.TServerSocket;
import org.apache.thrift.transport.TServerTransport;
import org.neo4j.graphdb.GraphDatabaseService;
import org.neo4j.graphdb.RelationshipType;
import org.neo4j.graphdb.Transaction;
import org.neo4j.kernel.api.exceptions.Status;
import org.slf4j.Logger;

import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.InputStream;
import java.util.*;
import java.util.concurrent.Callable;
import java.util.concurrent.CompletableFuture;
import java.util.function.Consumer;

/**
 * Created with IntelliJ IDEA.
 * User: apoorvkul
 * Date: 2/3/14
 * Time: 9:53 PM
 * To change this template use File | Settings | File Templates.
 */

public class Utils {

    public static<T> T getValue(Optional<T> optVal, T def)
    {
        return optVal.isPresent() ? optVal.get() : def;
    }

    public static enum RelTypes implements RelationshipType {
        IN_WAY_OF, // Node in fastest way given by directions API
        IN_POOLED_WAY_OF,    // Node in way through all pickup/drop-off points for the pool
        IN_REAL_WAY_OF,      // Node in actual way taken by car (in case driver changes the route on the way)
        BOOKED
    }

    public static Properties loadProperties(Class cls) throws IOException {
        String filename = String.format("%s.properties", cls.getSimpleName());
        InputStream input = cls.getClassLoader().getResourceAsStream(filename);
        if(input==null){
            throw new FileNotFoundException(String.format("File Not Found: %s", filename));
        }
        Properties prop = new Properties();
        prop.load(input);
        return prop;
    }

    public static void runThriftServer(Logger logger, TProcessor processor, int port)
    {
        Runnable service = () -> {
            try {
                TServerTransport serverTransport = new TServerSocket(port);
                TServer server = new TSimpleServer(new TServer.Args(serverTransport).processor(processor));
                // Use this for a multithreaded server
                // TServer server = new TThreadPoolServer(new TThreadPoolServer.Args(serverTransport).processor(processor));

                logger.info("Starting listening on " + port);
                server.serve();
            } catch (Exception e) {
                logger.error(e.toString());
            }
        };

        new Thread(service).start();
    }

    public static CompletableFuture<String> downloadStringAsync(Logger logger, String url)
    {
        return CompletableFuture.supplyAsync(() -> {
            try {
                return Request.Get(url).execute().returnContent().asString();
            } catch (IOException e) {
                logger.error(e.toString());
                return null;
            }
        });
    }
}
