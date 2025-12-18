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
ENV ASPNETCORE_URLS=http://0.0.0.0:5002
EXPOSE 5002
ENTRYPOINT ["dotnet", "ItemsService.dll"]