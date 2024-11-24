CREATE DATABASE legrand_db;

\connect legrand_db

CREATE TABLE IF NOT EXISTS WomenItems (
  id SERIAL PRIMARY KEY,
  brand VARCHAR(255),
  model VARCHAR(255),
  color VARCHAR(255),
  size VARCHAR(255),
  price FLOAT,
  avatar VARCHAR(255),
  description TEXT,
  manufacturerId INT
);

CREATE TABLE IF NOT EXISTS MenItems (
  id SERIAL PRIMARY KEY,
  brand VARCHAR(255),
  model VARCHAR(255),
  color VARCHAR(255),
  size VARCHAR(255),
  price FLOAT,
  avatar VARCHAR(255),
  description TEXT,
  manufacturerId INT
);

CREATE TABLE IF NOT EXISTS Manufacturers (
	id SERIAL PRIMARY KEY,
	name VARCHAR(255),
	country VARCHAR(255),
	address TEXT
);