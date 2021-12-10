#!/bin/bash

SCRIPT_PATH="$(dirname "$(realpath "$0")")"
ROOT_PATH="$SCRIPT_PATH/../.."
cd $ROOT_PATH

docker build -t dcapi . -f ./buildScripts/Api/Dockerfile-Base
docker build -t dcapi-demo-plugin . -f ./buildScripts/Api/Dockerfile-DemoPlugin
docker build -t dcbot . -f ./buildScripts/Bot/Dockerfile 
# docker build -t dcapi-matrix-plugin . -f ./buildScripts/Api/Dockerfile-NumberMatrix

docker tag dcapi jkamsker/coding-challenge-api:latest-base
docker tag dcapi-demo-plugin jkamsker/coding-challenge-api:latest-demo-plugin


