FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory
WORKDIR /app

# Copy the project files
COPY . .

# Restore and build the project
RUN dotnet restore
RUN dotnet build
RUN dotnet publish -c Release -o out

# Final stage: run the application using the ASP.NET Core runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Expose port 80 for the application
EXPOSE 5000

# Set the application to listen on IPv4 only
ENV ASPNETCORE_URLS=http://0.0.0.0:5000
ENV WEBSITES_ENABLE_APP_SERVICE_STORAGE false

RUN ls -l
# Run the application
ENTRYPOINT ["dotnet", "eras-be.sln"]