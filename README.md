# Overview

This is a program which emulates fault-tolerant in-memory distirbuted log. The messages replicated in parallel (To introduce delay Secondary2 - delays response for 3 seconds)


# Running the App

```bash
    # use docker-compose
    # from the root run the following command
    docker-compose up --build
```

# Using the App

```bash
    # post a message (master only)
    POST localhost:9091/log 
    {"Message": "Test"}

    # get logs from master
    GET localhost:9091/log
    # get logs from secondary1
    GET localhost:9092/log
    # get logs from secondary2
    GET localhost:9093/log

```
# Technologies

.NET Core 3.1, Docker
