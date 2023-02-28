FROM mcr.microsoft.com/dotnet/sdk:6.0 AS installer-env

# Install nuget credentials provider
ARG PAT
RUN wget -qO- https://raw.githubusercontent.com/Microsoft/artifacts-credprovider/master/helpers/installcredprovider.sh | bash
ENV NUGET_CREDENTIALPROVIDER_SESSIONTOKENCACHE_ENABLED true
ENV VSS_NUGET_EXTERNAL_FEED_ENDPOINTS "{\"endpointCredentials\": [{\"endpoint\":\"https://pkgs.dev.azure.com/apetito/_packaging/apetitoFeed/nuget/v3/index.json\", \"password\":\"${PAT}\"}]}"



COPY . /src/dotnet-function-app

RUN dotnet restore "/src/dotnet-function-app/apetito.meinapetito.Webhooks.ArticleChanges.Api/apetito.meinapetito.Webhooks.ArticleChanges.Api.csproj" -s "https://pkgs.dev.azure.com/apetito/_packaging/apetitoFeed/nuget/v3/index.json" -s "https://api.nuget.org/v3/index.json" 

RUN cd /src/dotnet-function-app && \
    mkdir -p /home/site/wwwroot && \
    dotnet publish apetito.meinapetito.Webhooks.ArticleChanges.Api/apetito.meinapetito.Webhooks.ArticleChanges.Api.csproj --output /home/site/wwwroot

# To enable ssh & remote debugging on app service change the base image to the one below
# FROM mcr.microsoft.com/azure-functions/dotnet-isolated:3.0-dotnet-isolated5.0-appservice
FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated6.0
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true

COPY --from=installer-env ["/home/site/wwwroot", "/home/site/wwwroot"]