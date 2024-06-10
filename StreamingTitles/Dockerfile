#Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source
COPY . .
RUN dotnet restore "./StreamingTitles.Api/StreamingTitles.Api.csproj" --disable-parallel
RUN dotnet publish "./StreamingTitles.Api/StreamingTitles.Api.csproj" -c release -o /app --no-restore

#Serve Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .

EXPOSE 5192
ENTRYPOINT ["dotnet", "StreamingTitles.Api.dll"]
