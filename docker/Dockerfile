FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ArrayCMS/ ./
RUN dotnet publish "ArrayCMS.csproj" -c Release -o /publish

FROM base AS final
WORKDIR /app
COPY --from=build /publish .
COPY ArrayCMS/appsettings.Development.json ./appsettings.Development.json

ENTRYPOINT ["dotnet", "ArrayCMS.dll"]
