FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/EventScheduler.Domain/EventScheduler.Domain.csproj src/EventScheduler.Domain/
COPY src/EventScheduler.Application/EventScheduler.Application.csproj src/EventScheduler.Application/
COPY src/EventScheduler.Infrastructure/EventScheduler.Infrastructure.csproj src/EventScheduler.Infrastructure/
COPY src/EventScheduler.API/EventScheduler.API.csproj src/EventScheduler.API/

RUN dotnet restore src/EventScheduler.API/EventScheduler.API.csproj

COPY src/ src/

RUN dotnet publish src/EventScheduler.API/EventScheduler.API.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "EventScheduler.API.dll"]