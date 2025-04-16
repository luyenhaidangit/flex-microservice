# Scratch Notes

## Ideas and Notes

- **Feature Enhancement**: Consider adding a caching layer to the Trade API to improve response times for frequently accessed data.
- **Refactoring**: Evaluate the current architecture for the Portfolio Management module to simplify dependencies and improve testability.

## Commands and Shortcuts

- **Docker Commands**:
  - Start PostgreSQL:
    ```bash
    docker run -e POSTGRES_PASSWORD=mysecretpassword -p 5432:5432 postgres
    ```
  - Start RabbitMQ:
    ```bash
    docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
    ```

- **.NET Commands**:
  - Run a specific microservice:
    ```bash
    dotnet run --project ./TradeService/TradeService.csproj
    ```
  - Run all unit tests:
    ```bash
    dotnet test
    ```

## Migration Command

- Tools > Nuget Package Manager > Package Manager Console > Default Project: Data
- Add-Migration Example
- Update-Database
- Remove-Migration
- Script-Migration
- Drop-Database

## Temporary Design Notes

- **Authentication Flow**:
  - Implement OAuth2 for enhanced security in user authentication.
  - Add rate limiting to login endpoints to prevent brute-force attacks.

- **Potential Tech Debt**:
  - Remove hard-coded API URLs from the codebase. Instead, move them to the configuration files or environment variables.

## Miscellaneous

- **Pending Research**:
  - Evaluate the use of [MassTransit](https://masstransit-project.com/) versus [NServiceBus](https://particular.net/nservicebus) for message brokering.
  - Investigate best practices for implementing retries in RabbitMQ consumers to handle intermittent failures.

## Useful Links

- [Official .NET 8 Documentation](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8)
- [Docker Best Practices](https://docs.docker.com/develop/best-practices/)
- [MassTransit Documentation](https://masstransit-project.com/documentation/)

Feel free to add more notes or remove anything that becomes outdated.

## Common nuget
- Flex.Swashbuckle: <PackageVersion Include="Swashbuckle.AspNetCore" Version="7.2.0" />
- Flex.Serilog: <PackageReference Include="Serilog.AspNetCore" Version="9.0.0" />

## Init Securities DB in Oracle
- CREATE USER luyenhaidangit IDENTIFIED BY haidang106;
- GRANT DBA TO luyenhaidangit;

## Init Investor DB in Oracle
- CREATE USER luyenhaidangit IDENTIFIED BY haidang106;
- GRANT DBA TO luyenhaidangit;

## Init Identity DB in Oracle
- CREATE USER luyenhaidangit IDENTIFIED BY haidang106;
- GRANT DBA TO luyenhaidangit;

## Migrate database identity server
Add-Migration InitIdentityDb -Context IdentityDbContext -Project Flex.IdentityServer.Api -StartupProject Flex.IdentityServer.Api
Add-Migration InitConfigDb -Context ConfigurationDbContext -Project Flex.IdentityServer.Api -StartupProject Flex.IdentityServer.Api
Add-Migration InitPersistedGrantDb -Context PersistedGrantDbContext -Project Flex.IdentityServer.Api -StartupProject Flex.IdentityServer.Api

Update-Database -Context IdentityDbContext -Project Flex.IdentityServer.Api -StartupProject Flex.IdentityServer.Api
Update-Database -Context ConfigurationDbContext -Project Flex.IdentityServer.Api -StartupProject Flex.IdentityServer.Api
Update-Database -Context PersistedGrantDbContext -Project Flex.IdentityServer.Api -StartupProject Flex.IdentityServer.Api
