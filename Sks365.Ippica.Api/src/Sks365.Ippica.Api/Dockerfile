#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS base
LABEL Maintainer="dvl-gaming@sks365.com"
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
#Copy all the content from build dir to the WORKDIR
#Build dir is a folder where the command build is executed
COPY . . 

# Add private nuget server
RUN dotnet nuget add source http://tc.corp.sks365.com/guestAuth/app/nuget/feed/_Root/default/v2

# Restore project to get all needed dependencies (libraries)
RUN dotnet restore src/Sks365.Ippica.Api/*.csproj

# Publish application to publish folder
WORKDIR "src/Sks365.Ippica.Api"
#RUN dotnet build "Sks365.Ippica.Api.csproj" -c Release -o /app/build
RUN dotnet publish *.csproj -c Release -o /app/publish

FROM build AS publish
RUN dotnet publish "Sks365.Ippica.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
## Copy custom configs to the WORKDIR
#COPY /Sks365.Ippica.Api/LogConfiguration.xml . 
# Copy binaries to the WORKDIR
COPY --from=publish /app/publish . 
ENTRYPOINT ["dotnet", "Sks365.Ippica.Api.dll"]