FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0 AS base

ARG RESOURCE_REAPER_SESSION_ID="00000000-0000-0000-0000-000000000000"
LABEL "org.testcontainers.resource-reaper-session"=$RESOURCE_REAPER_SESSION_ID

WORKDIR /home/site/wwwroot
EXPOSE 80

FROM mcr.microsoft.com/dotnet/runtime:8.0 as runtime8
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

COPY --from=runtime8 /usr/share/dotnet/host /usr/share/dotnet/host
COPY --from=runtime8 /usr/share/dotnet/shared /usr/share/dotnet/shared
WORKDIR /src

COPY ["nuget.config", "."]
COPY ["Commerce.Functions.Isolated/Commerce.Functions.Isolated.csproj", "Commerce.Functions.Isolated/"]
COPY ["Commerce.Business/Commerce.Business.csproj", "Commerce.Business/"]
COPY ["Commerce.Domain/Commerce.Domain.csproj", "Commerce.Domain/"]
COPY ["Commerce.Infrastructure/Commerce.Infrastructure.csproj", "Commerce.Infrastructure/"]
RUN dotnet restore "Commerce.Functions.Isolated/Commerce.Functions.Isolated.csproj"
COPY . .

WORKDIR "/src/Commerce.Functions.Isolated"

RUN dotnet build "Commerce.Functions.Isolated.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Commerce.Functions.Isolated.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /home/site/wwwroot
COPY --from=publish /app/publish .