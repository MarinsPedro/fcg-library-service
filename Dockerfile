# syntax=docker/dockerfile:1.7

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN --mount=type=secret,id=nuget_config,target=/root/.nuget/NuGet/NuGet.Config,required=true \
    dotnet restore FiapCloudGames.Library.slnx --configfile /root/.nuget/NuGet/NuGet.Config
RUN dotnet publish src/FiapCloudGames.Library.Api/FiapCloudGames.Library.Api.csproj -c Release --no-restore -o /app/api
RUN dotnet publish src/FiapCloudGames.Library.Worker/FiapCloudGames.Library.Worker.csproj -c Release --no-restore -o /app/worker
RUN dotnet publish src/FiapCloudGames.Library.Migrations/FiapCloudGames.Library.Migrations.csproj -c Release --no-restore -o /app/migrations

FROM runtime AS migrations
WORKDIR /app
COPY --from=build /app/migrations .
ENTRYPOINT ["dotnet", "FiapCloudGames.Library.Migrations.dll"]

FROM runtime AS worker
WORKDIR /app
COPY --from=build /app/worker .
ENTRYPOINT ["dotnet", "FiapCloudGames.Library.Worker.dll"]

FROM runtime AS final
WORKDIR /app
COPY --from=build /app/api .
ENTRYPOINT ["dotnet", "FiapCloudGames.Library.Api.dll"]
