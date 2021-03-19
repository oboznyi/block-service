# block-service

The point of this service is to show how .net core applications work with RabbitMQ, Redis and Postgresql in docker via docker-compose file.

# Description
Imitation of a mining system, namely block processing

# Block.Sender 
Sends last block number each 10 seconds to RabbitMQ. You can configure time thow enviroment variables.

# Block.Processing
Handling blocks:
  - Inserting blocks to DB
  - Incrementing count of blocks in Redis

# Configuration
  RabbitMQ:
  - Configuring with env_file(path ./config/rmq/.env.rmq)
  - Ports 5672, 15672
  Postgresql:
  - Configuring with env_file(path ./config/pg/.env)
  - Port 5432
  Redis:
  - Port 6379
  Block.Sender:
    Environment variables 
     - RMQ_CONNECTION: connection string to rabbitmq
     - BLOCK_DELAY_IN_SECONDS: Time to publich new block
  Block.Processing:
    Environment variables 
     - RMQ_CONNECTION: connection string to rabbitmq
     - DB_CONNECTION: connection string to postgresql db
     - REDIS_CONNECTION: connection string to redis
# Usage

Start the containers

$ docker-compose up -d
