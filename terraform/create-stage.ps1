# login with admin user!
az login 

cd root/base
terraform init -backend-config="stage/backend.tfvars" -reconfigure
terraform import -var-file="stage/env.tfvars" module.serviceBusNamespace.azurerm_key_vault_secret.meinapetito_azureservicebusnamespace_primaryconnectionstring "https://apetitokeyvault-staging.vault.azure.net/secrets/apetito-meinapetito-azureservicebusnamespace-primaryconnectionstring/d4ae33d205ac45759cfda2755314c6de" 

./create-stage.ps1

#
# Initialize databases (migrations)
#

# 1. create firewall rule for client ip in sql server
# 2. git clone Authorization / Notification

# Authorization
cd \
cd git\apetito.Authorization\apetito.Authorization.Data
$authorizationConnectionString = $(az keyvault secret show --vault-name "apetitoKeyVault-staging" --name "apetito-authorization-api-ConnectionStrings-AuthorizationDbConnectionString-meinapetito" --query "value")
.\UpdateDatabases.ps1 $authorizationConnectionString "AuthorizationContext" # retruns an error the first time being called
.\UpdateDatabases.ps1 $authorizationConnectionString "AuthorizationContext"

cd \
cd git\apetito.meinapetito
# SOmehow
# invoke-sqlcmd -connectionString $authorizationConnectionString -inputFile portal\api\src\apetito.meinapetito.Portal.Data\Root\Users\Permissions\Create-meinapetito-permissions.sql

# Notification


cd \
cd .\git\apetito.Notification\apetito.Notification.Infrastructure\
$notificationConnectionString = $(az keyvault secret show --vault-name "apetitoKeyVault-staging" --name "apetito-notification-api-ConnectionStrings-NotificationContext-meinapetito" --query "value")
.\UpdateDatabases.ps1 $notificationConnectionString 

cd \
cd git\apetito.meinapetito
# SOmehow
# invoke-sqlcmd -inputFile services/apetito.Notification/infrastructure/CreateConfiguration-Staging.sql

# Distributed Cache
cd \
cd git\apetito.meinapetito
cd service\DistributedCache\infrastructure\
$distributedCacheConnectionString = $(az keyvault secret show --vault-name "apetitoKeyVault-staging" --name "apetito-authorization-api-ConnectionStrings-DistributedCache-meinapetito" --query "value")
# somehow
# install-dotnet-sql-cache.ps1

# Portal DB
cd \
cd git\apetito.meinapetito
cd portal\api\src\apetito.meinapetito.Portal.Data
$portalDbConnectionString = "Server=tcp:stage-meinapetito.database.windows.net,1433;Initial Catalog=PortalDb;Persist Security Info=False;User ID=enchantingshortbread;Password=<value omitted>;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;" # somehow: grap app setting and replace placeholders for user / password from kv
.\UpdateDatabases.ps1 $portalDbConnectionString

#
# Create application gateway
#

cd..
cd applicationGateway
./create-stage.ps1

# Set DNS Entry (public application gateway ip to mein.apetito.de)

cd..
cd cluster
./create-stage.ps1
./createIngressController.ps1
./allowServiceBusInWindowsFirewall.ps1
# TODO: Create explicit backend pool? Add target (private cluster lb ip to backend pool)

# Create all app settings
# create secret for app settings user


# Deploy all images
cd \
cd git\apetito.meinapetito
.\deploay-staging.ps1