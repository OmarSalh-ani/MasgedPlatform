# Build from repo root: docker build -f AdminAPI/Dockerfile .
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Masged.WhatsApp/Masged.WhatsApp.csproj Masged.WhatsApp/
COPY AdminAPI/AdminAPI.csproj AdminAPI/
RUN dotnet restore AdminAPI/AdminAPI.csproj

COPY Masged.WhatsApp/ Masged.WhatsApp/
COPY AdminAPI/ AdminAPI/
WORKDIR /src/AdminAPI
RUN dotnet publish AdminAPI.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

RUN mkdir -p /app/Uploads /app/FilesManager /app/Logs /app/static/qcf-fonts

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AdminAPI.dll"]
