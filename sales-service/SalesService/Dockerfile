FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app .
# Ensure writable data directory for SQLite
RUN mkdir -p /app/data && chmod -R 777 /app/data
# Configure service URLs via environment later
ENV ASPNETCORE_URLS=http://0.0.0.0:5003
EXPOSE 5003
ENTRYPOINT ["dotnet", "SalesService.dll"]