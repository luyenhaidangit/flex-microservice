# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy solution file và restore các dependency
COPY *.sln .
COPY src/Services/Securities/Flex.Securities.Api/*.csproj ./Flex.Securities.Api/
RUN dotnet restore

# Copy toàn bộ source code
COPY . .

# Build project
RUN dotnet publish src/Services/Securities/Flex.Securities.Api/Flex.Securities.Api.csproj -c Release -o /app/out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copy từ stage build sang stage runtime
COPY --from=build /app/out .

# Mở cổng 5000
EXPOSE 5000

# Chạy ứng dụng
ENTRYPOINT ["dotnet", "Flex.Securities.Api.dll"]
