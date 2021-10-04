# Overview

This is a program which emulates in-memory replicated log. 
.NET Core 3.1, Docker

# Running the App

```bash
    # use docker-compose
    # from the root run the following command
    docker-compose up --build
```

# Using the App

```bash
    # get logs from master
    GET localhost:9091/log
    # get logs from secondary1
    GET localhost:9092/log
    # get logs from secondary2
    GET localhost:9093/log

    # post a message (master only)
    POST localhost:8080/log 
    {"Message": "Test"}
```
