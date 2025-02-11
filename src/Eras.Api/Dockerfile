# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80
EXPOSE 443

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Eras.Api/Eras.Api.csproj", "src/Eras.Api/"]
RUN dotnet restore "./src/Eras.Api/Eras.Api.csproj"
COPY . .
WORKDIR "/src/src/Eras.Api"
RUN dotnet build "./Eras.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Eras.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app

# Install PostgreSQL client in the final image (if needed in production)
USER root
RUN apt-get update && apt-get install -y postgresql-client && mkdir -p /https && chown -R $APP_UID:$APP_UID /https && chmod 755 /https && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .
USER $APP_UID
# Set the ASPNETCORE_ENVIRONMENT to Production by default - Github workflow
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Eras.Api.dll"]