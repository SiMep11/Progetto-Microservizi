# Fase di esecuzione
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Fase di compilazione
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

# Dichiarazione delle variabili passate da GitHub Actions
ARG GH_TOKEN
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiamo PRIMA il file di configurazione NuGet
COPY ["nuget.config", "."]

# Copiamo SOLO i file di progetto per sfruttare la cache di Docker
COPY ["Approvvigionamenti.WebApi/Approvvigionamenti.WebApi.csproj", "Approvvigionamenti.WebApi/"]
COPY ["Approvvigionamenti.Business/Approvvigionamenti.Business.csproj", "Approvvigionamenti.Business/"]
COPY ["Approvvigionamenti.Repository/Approvvigionamenti.Repository.csproj", "Approvvigionamenti.Repository/"]
COPY ["Approvvigionamenti.Shared/Approvvigionamenti.Shared.csproj", "Approvvigionamenti.Shared/"]
COPY ["Approvvigionamenti.ClientHttp/Approvvigionamenti.ClientHttp.csproj", "Approvvigionamenti.ClientHttp/"]

# Grazie al nuget.config e ad ARG GH_TOKEN, scarica anche i pacchetti privati da GitHub
RUN dotnet restore "./Approvvigionamenti.WebApi/Approvvigionamenti.WebApi.csproj"

# Ora copiamo tutto il resto del codice sorgente
COPY . .

WORKDIR "/src/Approvvigionamenti.WebApi"
RUN dotnet build "./Approvvigionamenti.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Fase di pubblicazione
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Approvvigionamenti.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Fase finale: immagine leggera per la produzione
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Approvvigionamenti.WebApi.dll"]