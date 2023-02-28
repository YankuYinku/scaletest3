$prevPwd = $PWD; Set-Location -ErrorAction Stop -LiteralPath $PSScriptRoot

. "./../../../infrastructure/scripts/create-appSettings.ps1"

$APPCONFIG_NAME = "apetitoAppSettings-development"; 
$KEYVAULT_NAME = "apetitoKeyVault-dev"; 


CreateSecret "apetito:authorization:api:ConnectionStrings:AuthorizationDbConnectionString" "<value omitted>" "meinapetito"
CreateSecret "apetito:authorization:api:ConnectionStrings:DistributedCache" "<Value omitted>" "meinapetito"
# This is not needed because we pass this in as an env variable because the transport connection string varies for each version of a cluster
# CreateSecret "apetito:authorization:api:ConnectionStrings:NServiceBusTransportConnectionString" "host=rabbitmq;username=guest;password=guest" "meinapetito"
CreateSecret "apetito:authorization:api:ConnectionStrings:NServiceBusPersistenceConnectionString" "<value omitted>" "meinapetito"


$prevPwd | Set-Location

