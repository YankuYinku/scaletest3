$prevPwd = $PWD; Set-Location -ErrorAction Stop -LiteralPath $PSScriptRoot

. "./../../../infrastructure/scripts/create-appSettings.ps1"

$APPCONFIG_NAME = "apetitoAppSettings";
$KEYVAULT_NAME = "apetitoKeyVault";

CreateSecret "apetito:notification:api:ConnectionStrings:NotificationContext" "<value omitted>" "meinapetito"
CreateSettingWithExistingKeyVaultEntry "apetito:notification:api:ConnectionStrings:NServiceBusTransportConnectionString" "apetito-meinapetito-azureservicebusnamespace-primaryconnectionstring" "meinapetito"
CreateSecret "apetito:notification:api:ConnectionStrings:NServiceBusPersistenceConnectionString" "<value omitted>" "meinapetito"
CreateSecret "apetito:notification:api:SendGridSettings:ApiKey" "<Value omitted>" "meinapetito"

$prevPwd | Set-Location
