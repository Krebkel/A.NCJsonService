FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
WORKDIR /build
COPY . .
RUN dotnet restore
RUN dotnet publish --property:PublishDir=/app/publish --no-restore --configuration Release

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS run
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "Web.dll"]