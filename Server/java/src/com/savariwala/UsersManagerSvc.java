/**
 * Created with IntelliJ IDEA.
 * User: apoorvkul
 * Date: 2/3/14
 * Time: 4:05 PM
 * To change this template use File | Settings | File Templates.
 */
package com.savariwala;

import org.apache.thrift.TException;
import org.apache.thrift.TProcessor;
import org.apache.thrift.server.TServer;
import org.apache.thrift.server.TSimpleServer;
import org.apache.thrift.transport.TServerSocket;
import org.apache.thrift.transport.TServerTransport;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.sql.*;
import java.util.Properties;

public class UsersManagerSvc implements com.savariwala.UsersManager.Iface {

    private PreparedStatement _prepareGetUser = null;
    private PreparedStatement _prepareAddUser = null;
    private Connection _conn = null;
    private  final Logger _logger = LoggerFactory.getLogger(this.getClass().getName());

    UsersManagerSvc() {
        try {
            Class.forName("org.postgresql.Driver");
            String url = "jdbc:postgresql:rideshare?user=rider&password=rider";
            _conn = DriverManager.getConnection(url);
            _prepareGetUser = _conn.prepareStatement(
                    "SELECT user_name, is_passenger FROM users WHERE fb_user_id = ?");
            _prepareAddUser = _conn.prepareStatement("INSERT INTO users values(?,?,?)");
        } catch (ClassNotFoundException e) {
            _logger.error(e.toString());
        } catch (SQLException e) {
            _logger.error(e.toString());
        }
    }

    @Override
    public User getUser(String fbUserId) throws TException {
        try {
            _prepareGetUser.setString(1, fbUserId);
            ResultSet rs = _prepareGetUser.executeQuery();
            if (!rs.next() ) {
                throw new ServerError(String.format("User Not Found: %s", fbUserId),
                        ErrorCode.NotFound);
            }
            User user = new User();
            user.setFbUserId(fbUserId);
            user.setUserName(rs.getString("user_name"));
            user.setIsPassenger(rs.getBoolean("is_passenger"));

            return user;

        } catch (SQLException e) {
            throw new ServerError(e.toString(), ErrorCode.Unspecified);
        }
    }

    @Override
    public void addUser(User user) throws TException {
        try {
            _prepareAddUser.setString(1, user.getFbUserId());
            _prepareAddUser.setString(2, user.getUserName());
            _prepareGetUser.setBoolean(3, user.isIsPassenger());
            _prepareAddUser.executeUpdate();
        } catch (SQLException e) {
            throw new ServerError(e.toString(), ErrorCode.Unspecified);
        }
    }

    public static void main(String [] args) {
        Logger logger = LoggerFactory.getLogger("Main");

        try {
            Properties prop = Utils.loadProperties(UsersManagerSvc.class);
            Utils.runThriftServer(logger,
                    new UsersManager.Processor(new UsersManagerSvc()),
                    Integer.parseInt(prop.getProperty("port", "9090")));

        } catch (Exception x) {
            logger.error(x.toString());
        }
    }

}
