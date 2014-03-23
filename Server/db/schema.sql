CREATE DATABASE rideshare
  WITH OWNER = postgres
       ENCODING = 'UTF8'
       TABLESPACE = pg_default
       LC_COLLATE = 'C'
       LC_CTYPE = 'C'
       CONNECTION LIMIT = -1;

CREATE TABLE Users(fb_user_id varchar(200), user_name varchar(200), is_passenger boolean);
CREATE TABLE map_points(latitude double precision, longitude double precision, name varchar(200), address varchar(300), locality varchar(100), is_src boolean default true, is_dst boolean default true);

INSERT INTO map_points VALUES (1.30037, 103.86065, 'ParkRoyal Lobby, Beach Rd', '7500A Beach Rd, Singapore 199591', 'Beach Rd');
INSERT INTO map_points VALUES (1.29945, 103.86031, 'Pan Pacific Lobby', '7500A Beach Rd, Singapore 199591', 'Beach Rd');
INSERT INTO map_points VALUES (1.29964, 103.86137, 'ParkRoyal Lobby, Nicoll Highway', '7500A Beach Rd, Singapore 199591', 'Beach Rd');
INSERT INTO map_points VALUES (1.28138, 103.85228, 'One Raffles Quay Lobby', '1 Raffes Quay, Singapore 048583', 'Raffles Quay');
INSERT INTO map_points VALUES (1.27991, 103.85395, 'Marina Bay Financial Centre Tower 2 Lobby', '10 Marina Boulevard, Singapore 018983', 'Marina Bay');
INSERT INTO map_points VALUES (1.27906, 103.85478, 'Marina Bay Financial Centre Tower 3 Lobby', '10 Marina Boulevard, Singapore 018983', 'Marina Bay');
