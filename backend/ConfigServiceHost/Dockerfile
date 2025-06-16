#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 4680

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ConfigServiceHost/ConfigServiceHost.csproj", "ConfigServiceHost/"]
COPY ["ConfigService/ConfigService.csproj", "ConfigService/"]
RUN dotnet restore "./ConfigServiceHost/./ConfigServiceHost.csproj"
COPY . .
WORKDIR "/src/ConfigServiceHost"
RUN dotnet build "./ConfigServiceHost.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ConfigServiceHost.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ConfigServiceHost.dll"]