FROM mcr.microsoft.com/dotnet/aspnet:8.0 as deploy-env
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app
COPY DragonApi.csproj src/DragonApi.csproj
RUN dotnet restore src/DragonApi.csproj
COPY . ./src
RUN dotnet publish src -c Release -o out

# Build runtime image
FROM deploy-env
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "DragonApi.dll"]