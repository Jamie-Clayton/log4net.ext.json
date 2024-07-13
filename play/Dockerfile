#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["play/play.csproj", "play/"]
COPY ["log4net.Ext.Json.Net/log4net.Ext.Json.Net.csproj", "log4net.Ext.Json.Net/"]
COPY ["log4net.Ext.Json/log4net.Ext.Json.csproj", "log4net.Ext.Json/"]
RUN dotnet restore "./play/play.csproj"
COPY . .
WORKDIR "/src/play"
RUN dotnet build "./play.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./play.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "play.dll"]