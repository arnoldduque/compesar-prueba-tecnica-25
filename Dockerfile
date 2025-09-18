# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["bibliaAPI/", "bibliaAPI/"]
COPY ["bibliaAPI-Tests/", "bibliaAPI-Tests/"]
RUN dotnet restore "./bibliaAPI/bibliaAPI.csproj"
RUN dotnet restore "./bibliaAPI-Tests/bibliaAPI-Tests.csproj"
COPY . .
WORKDIR "/src/bibliaAPI"
RUN dotnet build "./bibliaAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build
WORKDIR "/src/bibliaAPI-Tests"
RUN dotnet build "./bibliaAPI-Tests.csproj" -c $BUILD_CONFIGURATION -o /app/tests

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/bibliaAPI"
RUN dotnet publish "./bibliaAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Test stage
FROM build AS test
WORKDIR /src/bibliaAPI-Tests
RUN dotnet test --no-restore --no-build --verbosity normal

# Final runtime image
FROM base AS final
WORKDIR /app
COPY ["bibliaAPI/biblia.db", "biblia.db"]
RUN chmod a+w "/app/biblia.db"
RUN chmod a+w "/app"
# Install sqlite3 CLI tool
RUN apt-get update \
    && apt-get install -y --no-install-recommends sqlite3 \
    && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "bibliaAPI.dll"]