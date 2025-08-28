# Use the official .NET 9.0 runtime image as the base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

# Use the official .NET 9.0 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["Flex.Gateway.Yarp.csproj", "./"]
RUN dotnet restore "Flex.Gateway.Yarp.csproj"

# Copy the rest of the source code
COPY . .

# Build the application
RUN dotnet build "Flex.Gateway.Yarp.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "Flex.Gateway.Yarp.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Build the final runtime image
FROM base AS final
WORKDIR /app

# Create logs directory
RUN mkdir -p /app/logs

# Copy the published application
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5000;https://+:5001
ENV ASPNETCORE_ENVIRONMENT=Production

# Create a non-root user
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:5000/health || exit 1

ENTRYPOINT ["dotnet", "Flex.Gateway.Yarp.dll"]
