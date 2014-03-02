/**
 * Created with IntelliJ IDEA.
 * User: apoorvkul
 * Date: 2/3/14
 * Time: 4:05 PM
 * To change this template use File | Settings | File Templates.
 */
package com.sawariwala;

import com.savariwala.User;
import org.apache.thrift.TException;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.sql.*;
import java.util.logging.LogManager;

class UsersManagerHandler implements com.savariwala.UsersManager.Iface {

    private PreparedStatement prepareGetUser = null;
    private PreparedStatement prepareAddUser = null;
    private Connection conn = null;
    private  final Logger _logger = LoggerFactory.getLogger(this.getClass().getName());

    UsersManagerHandler() {
        try {
            Class.forName("org.postgresql.Driver");
            String url = "jdbc:postgresql:rideshare?user=rider&password=rider";
            conn = DriverManager.getConnection(url);
            prepareGetUser = conn.prepareStatement(
                    "SELECT user_name, is_passenger FROM users WHERE fb_user_id = ?");
            prepareAddUser = conn.prepareStatement("INSERT INTO users values(?,?,?)");
        } catch (ClassNotFoundException e) {
            _logger.error(e.toString());
        } catch (SQLException e) {
            _logger.error(e.toString());
        }
    }

    @Override
    public User getUser(String fbUserId) throws TException {
        try {
            prepareGetUser.setString(0, fbUserId);
            ResultSet rs = prepareGetUser.executeQuery();
            if (!rs.next() ) {
                throw new TException(String.format("User Not Found: %s", fbUserId));
            }
            User user = new User();
            user.setFbUserId(fbUserId);
            user.setUserName(rs.getString("user_name"));
            user.setIsPassenger(rs.getBoolean("is_passenger"));

            return user;

        } catch (SQLException e) {
            throw new TException(e.toString());
        }
    }

    @Override
    public void addUser(User user) throws TException {
        try {
            prepareAddUser.setString(0, user.getFbUserId());
            prepareAddUser.setString(1, user.getUserName());
            prepareGetUser.setBoolean(2, user.isIsPassenger());
            prepareAddUser.executeUpdate();
        } catch (SQLException e) {
            throw new TException(e.toString());
        }
    }
}
