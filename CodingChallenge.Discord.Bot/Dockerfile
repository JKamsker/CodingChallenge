# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app/Bot

# Copy csproj and restore as distinct layers
# COPY *.csproj ./
ADD ./CodingChallenge.Discord.Bot .
ADD ./external ../external


RUN dotnet restore

# Copy everything else and build
RUN dotnet publish -c Release -o ../out

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "CodingChallenge.Discord.Bot.dll"]

# Build runtime image
# FROM mcr.microsoft.com/dotnet/aspnet:6.0
# WORKDIR /app
# COPY --from=build-env /app/out .
# ENTRYPOINT ["dotnet", "aspnetapp.dll"]


#COPY * ./
#COPY ../external ./..
# RUN dotnet restore

# # Copy everything else and build
# COPY ../engine/examples ./
# RUN dotnet publish -c Release -o out




