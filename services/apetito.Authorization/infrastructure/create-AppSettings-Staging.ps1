$prevPwd = $PWD; Set-Location -ErrorAction Stop -LiteralPath $PSScriptRoot

. "./../../../infrastructure/scripts/create-appSettings.ps1"

$APPCONFIG_NAME = "apetitoAppSettings-staging"; 
$KEYVAULT_NAME = "apetitoKeyVault-staging";


CreateSecret "apetito:authorization:api:ConnectionStrings:AuthorizationDbConnectionString" "Server=tcp:stagingmeinapetitosqlserver.database.windows.net,1433;Initial Catalog=AuthorizationDb;Persist Security Info=False;User ID=enchantingshortbread;Password={Your Password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;" "meinapetito"
CreateSecret "apetito:authorization:api:ConnectionStrings:DistributedCache" "Server=tcp:stagingmeinapetitosqlserver.database.windows.net,1433;Initial Catalog=DistributedCache;Persist Security Info=False;User ID=enchantingshortbread;Password={Your Password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;" "meinapetito"
CreateSettingWithExistingKeyVaultEntry "apetito:authorization:api:ConnectionStrings:NServiceBusTransportConnectionString" "apetito-meinapetito-azureservicebusnamespace-primaryconnectionstring" "meinapetito"
CreateSecret "apetito:authorization:api:ConnectionStrings:NServiceBusPersistenceConnectionString" "Server=tcp:stagingmeinapetitosqlserver.database.windows.net,1433;Initial Catalog=NServiceBus;Persist Security Info=False;User ID=enchantingshortbread;Password={Your Password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;" "meinapetito"


$prevPwd | Set-Location

