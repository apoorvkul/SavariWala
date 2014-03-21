CREATE TABLE Users(fb_user_id varchar(200), user_name varchar(200), is_passenger boolean);
CREATE TABLE map_points(latitude double precision, longitude double precision, description varchar(200), is_src boolean default true, is_dst boolean default true);

INSERT INTO map_points VALUES (1.30037, 103.86065, 'ParkRoyal Lobby, Beach Rd');
INSERT INTO map_points VALUES (1.29945, 103.86031, 'Pan Pacific Lobby');
INSERT INTO map_points VALUES (1.29964, 103.86137, 'ParkRoyal Lobby, Nicoll Highway');
INSERT INTO map_points VALUES (1.28138, 103.85228, 'One Raffles Quay Lobby');
INSERT INTO map_points VALUES (1.27991, 103.85395, 'Marina Bay Financial Centre Tower 2 Lobby');
INSERT INTO map_points VALUES (1.27906, 103.85478, 'Marina Bay Financial Centre Tower 3 Lobby');
