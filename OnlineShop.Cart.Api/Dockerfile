#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 
ARG source
WORKDIR /app
COPY ${source:-obj/Docker/publist} .
ENTRYPOINT ["dotnet", "OnlineShop.Cart.Api.dll"]