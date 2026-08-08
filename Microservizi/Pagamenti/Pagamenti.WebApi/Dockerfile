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
COPY ["Pagamenti.WebApi/Pagamenti.WebApi.csproj", "Pagamenti.WebApi/"]
COPY ["Pagamenti.Business/Pagamenti.Business.csproj", "Pagamenti.Business/"]
COPY ["Pagamenti.Repository/Pagamenti.Repository.csproj", "Pagamenti.Repository/"]
COPY ["Pagamenti.Shared/Pagamenti.Shared.csproj", "Pagamenti.Shared/"]
COPY ["Pagamenti.ClientHttp/Pagamenti.ClientHttp.csproj", "Pagamenti.ClientHttp/"]

# Eseguiamo il restore direttamente sul progetto principale
# Grazie al nuget.config e ad ARG GH_TOKEN, scaricherà anche i tuoi pacchetti privati da GitHub!
RUN dotnet restore "./Pagamenti.WebApi/Pagamenti.WebApi.csproj"

# Ora copiamo tutto il resto del codice sorgente
COPY . .

# Ci spostiamo ed eseguiamo la build
WORKDIR "/src/Pagamenti.WebApi"
RUN dotnet build "./Pagamenti.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Fase di pubblicazione
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Pagamenti.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Fase finale: creazione dell'immagine leggera per la produzione
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pagamenti.WebApi.dll"]