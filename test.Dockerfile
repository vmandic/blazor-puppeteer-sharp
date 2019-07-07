FROM mcr.microsoft.com/dotnet/core/sdk:2.1 as build-step
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY test/PuppeteerE2E/*.csproj ./
RUN dotnet restore

# Installs latest Chromium (73) package.
FROM node:10-alpine as dl-chromium
WORKDIR /chromium-dl
RUN apk update && apk upgrade && \
    echo @edge http://nl.alpinelinux.org/alpine/edge/community >> /etc/apk/repositories && \
    echo @edge http://nl.alpinelinux.org/alpine/edge/main >> /etc/apk/repositories && \
    apk add --no-cache \
      chromium@edge=~73.0.3683.103 \
      nss@edge \
      freetype@edge \
      freetype-dev@edge \
      harfbuzz@edge \
      ttf-freefont@edge
RUN ["cp", "/usr/bin/chromium-browser", "/chromium-dl"]

# Copy everything else and build
FROM mcr.microsoft.com/dotnet/core/sdk:2.1 as run-step
WORKDIR /app
ENV chromium_path=/app/chromium/chromium-browser 
COPY --from=dl-chromium /chromium-dl /app/chromium
COPY test/PuppeteerE2E/ ./
ENTRYPOINT ["dotnet", "test", "PuppeteerE2E.csproj"]