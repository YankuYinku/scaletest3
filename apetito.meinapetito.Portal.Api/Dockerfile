FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Install nuget credentials provider
ARG PAT
RUN wget -qO- https://raw.githubusercontent.com/Microsoft/artifacts-credprovider/master/helpers/installcredprovider.sh | bash
ENV NUGET_CREDENTIALPROVIDER_SESSIONTOKENCACHE_ENABLED true
ENV VSS_NUGET_EXTERNAL_FEED_ENDPOINTS "{\"endpointCredentials\": [{\"endpoint\":\"https://pkgs.dev.azure.com/apetito/_packaging/apetitoFeed/nuget/v3/index.json\", \"password\":\"${PAT}\"}]}"


WORKDIR /src
COPY ["apetito.meinapetito.Portal.Api/apetito.meinapetito.Portal.Api.csproj", "apetito.meinapetito.Portal.Api/"]
COPY ["apetito.meinapetito.Portal.Application/apetito.meinapetito.Portal.Application.csproj", "apetito.meinapetito.Portal.Application/"]
COPY ["apetito.meinapetito.Portal.Contracts/apetito.meinapetito.Portal.Contracts.csproj", "apetito.meinapetito.Portal.Contracts/"]
COPY ["apetito.meinapetito.Portal.NServiceBusEndpoint/apetito.meinapetito.Portal.NServiceBusEndpoint.csproj", "apetito.meinapetito.Portal.NServiceBusEndpoint/"]
COPY ["apetito.meinapetito.Portal.Data/apetito.meinapetito.Portal.Data.csproj", "apetito.meinapetito.Portal.Data/"]

RUN dotnet restore "apetito.meinapetito.Portal.Api/apetito.meinapetito.Portal.Api.csproj" -s "https://pkgs.dev.azure.com/apetito/_packaging/apetitoFeed/nuget/v3/index.json" -s "https://api.nuget.org/v3/index.json" 

COPY . .
WORKDIR "/src/apetito.meinapetito.Portal.Api"

FROM build AS publish
RUN dotnet publish "apetito.meinapetito.Portal.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "apetito.meinapetito.Portal.Api.dll"]
