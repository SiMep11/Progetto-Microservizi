# Fase di esecuzione (il server nudo e crudo)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Fase di compilazione (l'SDK con tutti gli strumenti)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiamo l'intera cartella del Magazzino (tutti e 5 i progetti!)
COPY . .

# Ci spostiamo dentro il WebApi e compiliamo tutto
WORKDIR "/src/Magazzino.WebApi"
RUN dotnet restore "./Magazzino.WebApi.csproj"
RUN dotnet publish "./Magazzino.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Fase finale: prendiamo il risultato della compilazione e lo mettiamo nel server nudo
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Magazzino.WebApi.dll"]