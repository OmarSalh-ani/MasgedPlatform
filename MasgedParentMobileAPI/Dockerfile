# Build from repo root: docker build -f MasgedParentMobileAPI/Dockerfile .
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY MasgedParentMobileAPI/MasgedParentMobileAPI.csproj MasgedParentMobileAPI/
RUN dotnet restore MasgedParentMobileAPI/MasgedParentMobileAPI.csproj

COPY MasgedParentMobileAPI/ MasgedParentMobileAPI/
WORKDIR /src/MasgedParentMobileAPI
RUN dotnet publish MasgedParentMobileAPI.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

RUN mkdir -p /app/Uploads

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MasgedParentMobileAPI.dll"]
