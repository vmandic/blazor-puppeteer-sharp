FROM mcr.microsoft.com/dotnet/core/sdk:2.1 as build-step
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY test/PuppeteerE2E/*.csproj ./
RUN dotnet restore

# Installs latest Chromium (73) package.
# FROM node:10-alpine as dl-chromium
# WORKDIR /chromium-dl
# RUN apk update && apk upgrade && \
#     echo @edge http://nl.alpinelinux.org/alpine/edge/community >> /etc/apk/repositories && \
#     echo @edge http://nl.alpinelinux.org/alpine/edge/main >> /etc/apk/repositories && \
#     apk add --no-cache \
#       chromium@edge=~73.0.3683.103 \
#       nss@edge \
#       freetype@edge \
#       freetype-dev@edge \
#       harfbuzz@edge \
#       ttf-freefont@edge
# RUN ["cp", "/usr/bin/chromium-browser", "/chromium-dl"]
# RUN ["cp", "-r", "/usr/lib/chromium", "/chromium-dl"]

# Copy everything else and build
FROM mcr.microsoft.com/dotnet/core/sdk:2.1 as run-step
WORKDIR /app
# ENV chromium_path=/app/chromium/chromium-browser 
# COPY --from=dl-chromium /chromium-dl /app/chromium

RUN apt-get update && apt-get install -y lsb-core 
# RUN && apt upgrade -y
RUN add-apt-repository "deb http://archive.ubuntu.com/ubuntu precise main universe restricted multiverse"
RUN apt update
RUN apt install -y chromium-browser

# install manually all the missing libraries
RUN apt-get install -y gconf-service libasound2 libatk1.0-0 libcairo2 libcups2 libfontconfig1 libgdk-pixbuf2.0-0 libgtk-3-0 libnspr4 libpango-1.0-0 libxss1 fonts-liberation libappindicator1 libnss3 lsb-release xdg-utils

# install chrome
RUN wget https://dl.google.com/linux/direct/google-chrome-stable_current_amd64.deb
RUN dpkg -i google-chrome-stable_current_amd64.deb; apt-get -fy install

COPY test/PuppeteerE2E/ ./
# RUN ["cp", "-r", "/app/chromium/chromium", "/app"]
# RUN ["rm", "-rf", "/app/chromium/chromium"]
ENTRYPOINT ["dotnet", "test", "PuppeteerE2E.csproj"]