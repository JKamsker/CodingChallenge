#!/bin/bash
# echo "hello world"

cd ../..

docker build -t dcapi . -f ./buildScripts/Api/Dockerfile-Base
docker build -t dcapi-demo-plugin . -f ./buildScripts/Api/Dockerfile-DemoPlugin
docker build -t dcapi-matrix-plugin . -f ./buildScripts/Api/Dockerfile-NumberMatrix

docker tag dcapi jkamsker/coding-challenge-api:latest-base
docker tag dcapi-demo-plugin jkamsker/coding-challenge-api:latest-demo-plugin


