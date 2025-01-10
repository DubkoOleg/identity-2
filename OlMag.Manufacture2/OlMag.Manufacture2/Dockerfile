#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8350
EXPOSE 8351

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["OlMag.Manufacture2/OlMag.Manufacture2.csproj", "."]
RUN dotnet restore "OlMag.Manufacture2/OlMag.Manufacture2.csproj"

COPY ["OlMag.Manufacture2/OlMag.Manufacture2/", "."]
WORKDIR /src
RUN dotnet build "OlMag.Manufacture2/OlMag.Manufacture2.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OlMag.Manufacture2/OlMag.Manufacture2.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OlMag.Manufacture2/OlMag.Manufacture2.dll"]
