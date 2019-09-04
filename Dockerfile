FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /skidata-app

COPY skidata-app/*.csproj ./skidata-app
RUN dotnet restore

COPY skidata-app/. ./skidata-app/
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS runtime
WORKDIR /skidata-appCOPY --from=build /skidata-app/out ./
ENTRYPOINT ["dotnet", "skidata-app.dll"]