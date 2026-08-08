# Fase di esecuzione
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Fase di compilazione
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

# IL METODO DEL PROF: Variabili ARG dichiarate all'inizio della build
ARG GH_TOKEN
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiamo PRIMA il nuget.config
COPY ["nuget.config", "."]

# Copiamo SOLO i file di progetto (ottimizza la cache di Docker come fa il prof)
COPY ["Magazzino.WebApi/Magazzino.WebApi.csproj", "Magazzino.WebApi/"]
COPY ["Magazzino.Shared/Magazzino.Shared.csproj", "Magazzino.Shared/"]
COPY ["Magazzino.Business/Magazzino.Business.csproj", "Magazzino.Business/"]
COPY ["Magazzino.Repository/Magazzino.Repository.csproj", "Magazzino.Repository/"]
COPY ["Magazzino.ClientHttp/Magazzino.ClientHttp.csproj", "Magazzino.ClientHttp/"]

# Eseguiamo il restore direttamente sul progetto principale
RUN dotnet restore "./Magazzino.WebApi/Magazzino.WebApi.csproj"

# Ora copiamo tutto il resto del codice sorgente
COPY . .

# Ci spostiamo ed eseguiamo la build
WORKDIR "/src/Magazzino.WebApi"
RUN dotnet build "./Magazzino.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Magazzino.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Magazzino.WebApi.dll"]