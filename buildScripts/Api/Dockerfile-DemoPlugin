FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

ADD . .

WORKDIR /app/CodingChallenge.ChallengeServer.Plugins.Demo

RUN dotnet restore

# Copy everything else and build
RUN dotnet publish -c Release -o ../out

# Build runtime image
FROM dcapi

ENV ChallengeServer__RepositoryName=DemoApi

# WORKDIR /app
COPY --from=build-env /app/out /plugins