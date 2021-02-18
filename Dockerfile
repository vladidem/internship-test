FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app
COPY PageStatistics/PageStatistics.csproj PageStatistics/PageStatistics.csproj
RUN dotnet restore PageStatistics

COPY . ./
RUN dotnet publish PageStatistics -c Release -o Release

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/Release .
ENTRYPOINT ["dotnet", "PageStatistics.dll"]
