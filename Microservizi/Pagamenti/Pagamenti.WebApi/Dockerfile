# 1. Base per l'esecuzione (leggera)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

# 2. Base per la compilazione (contiene tutto l'SDK)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# COPIA IL NUGET.CONFIG PER SCARICARE DA GITHUB!
# (Assicurati che il file nuget.config sia nella cartella della Soluzione)
COPY ["nuget.config", "."]

# Copia i file di progetto (.csproj) per fare il restore
# NOTA: Assicurati che i nomi delle cartelle combacino col tuo progetto
COPY ["Pagamenti.WebApi/Pagamenti.WebApi.csproj", "Pagamenti.WebApi/"]
COPY ["Pagamenti.Business/Pagamenti.Business.csproj", "Pagamenti.Business/"]
COPY ["Pagamenti.Repository/Pagamenti.Repository.csproj", "Pagamenti.Repository/"]
COPY ["Pagamenti.ClientHttp/Pagamenti.ClientHttp.csproj", "Pagamenti.ClientHttp/"]
COPY ["Pagamenti.Shared/Pagamenti.Shared.csproj", "Pagamenti.Shared/"]

ARG GH_TOKEN
RUN dotnet nuget add source "https://nuget.pkg.github.com/simep11/index.json" --name "github" --username "simep11" --password "$GH_TOKEN" --store-password-in-clear-text

# Scarica le dipendenze usando il nuget.config appena copiato
RUN dotnet restore "Pagamenti.WebApi/Pagamenti.WebApi.csproj" --configfile nuget.config

# Copia tutto il resto del codice e compila
COPY . .
WORKDIR "/src/Pagamenti.WebApi"
RUN dotnet build "Pagamenti.WebApi.csproj" -c Release -o /app/build

# 3. Pubblicazione
FROM build AS publish
RUN dotnet publish "Pagamenti.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. Immagine finale pronta per girare
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pagamenti.WebApi.dll"]