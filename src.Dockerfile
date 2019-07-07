FROM mcr.microsoft.com/dotnet/core/sdk:3.0.100-preview6-alpine3.9 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY src/*.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY src/ ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.0.0-preview6-alpine3.9
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "BlazorServerSidePuppeteerSharp.dll"]
