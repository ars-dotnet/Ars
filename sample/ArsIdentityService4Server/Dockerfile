#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

#ENV ASPNETCORE_URLS https://+:5105

COPY . .

ENTRYPOINT ["dotnet", "ArsIdentityService4Server.dll"]