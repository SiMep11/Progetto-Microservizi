# Vedere https://aka.ms/customizecontainer per informazioni su come personalizzare il contenitore di debug e su come Visual Studio usa questo Dockerfile per compilare le immagini per un debug più rapido.

# Questa fase viene usata durante l'esecuzione da Visual Studio in modalità rapida (impostazione predefinita per la configurazione di debug)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Questa fase viene usata per compilare il progetto di servizio
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Ordini.WebApi/Ordini.WebApi.csproj", "Ordini.WebApi/"]
COPY ["Ordini.Business/Ordini.Business.csproj", "Ordini.Business/"]
COPY ["Ordini.ClientHttp/Ordini.ClientHttp.csproj", "Ordini.ClientHttp/"]
COPY ["Ordini.Shared/Ordini.Shared.csproj", "Ordini.Shared/"]
COPY ["Ordini.Repository/Ordini.Repository.csproj", "Ordini.Repository/"]

ARG GH_TOKEN
RUN dotnet nuget add source "https://nuget.pkg.github.com/simep11/index.json" --name "github" --username "simep11" --password "$GH_TOKEN" --store-password-in-clear-text

RUN dotnet restore "./Ordini.WebApi/Ordini.WebApi.csproj"
COPY . .
WORKDIR "/src/Ordini.WebApi"
RUN dotnet build "./Ordini.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Questa fase viene usata per pubblicare il progetto di servizio da copiare nella fase finale
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Ordini.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Questa fase viene usata nell'ambiente di produzione o durante l'esecuzione da Visual Studio in modalità normale (impostazione predefinita quando non si usa la configurazione di debug)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ordini.WebApi.dll"]