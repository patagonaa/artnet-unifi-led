FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app/src
# copy csproj only so restored project will be cached
COPY src/ArtnetUnifiLed/ArtnetUnifiLed.csproj /app/src/ArtnetUnifiLed/
RUN dotnet restore ArtnetUnifiLed/ArtnetUnifiLed.csproj
COPY src/ /app/src
RUN dotnet publish -c Release ArtnetUnifiLed/ArtnetUnifiLed.csproj -o /app/build

FROM mcr.microsoft.com/dotnet/core/runtime:3.1
WORKDIR /app
COPY --from=build /app/build/ ./
ENTRYPOINT ["dotnet", "ArtnetUnifiLed.dll"]