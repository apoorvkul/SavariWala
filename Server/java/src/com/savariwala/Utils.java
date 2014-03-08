package com.savariwala;

import org.apache.thrift.TProcessor;
import org.apache.thrift.server.TServer;
import org.apache.thrift.server.TSimpleServer;
import org.apache.thrift.transport.TServerSocket;
import org.apache.thrift.transport.TServerTransport;
import org.slf4j.Logger;

import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.InputStream;
import java.util.Properties;

/**
 * Created with IntelliJ IDEA.
 * User: apoorvkul
 * Date: 2/3/14
 * Time: 9:53 PM
 * To change this template use File | Settings | File Templates.
 */
public class Utils {
    public static Properties loadProperties(Class cls) throws IOException {
        String filename = String.format("%s.properties", cls.getName());
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

                logger.info("Starting UserManager on 9090");
                server.serve();
            } catch (Exception e) {
                logger.error(e.toString());
            }
        };

        new Thread(service).start();
    }
}
