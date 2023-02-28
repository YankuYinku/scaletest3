$prevPwd = $PWD; Set-Location -ErrorAction Stop -LiteralPath $PSScriptRoot

. "./../../../infrastructure/scripts/create-appSettings.ps1"

$APPCONFIG_NAME = "apetitoAppSettings-development"; 
$KEYVAULT_NAME = "apetitoKeyVault-dev"; 


CreateSecret "apetito:notification:api:ConnectionStrings:NotificationContext" "<value omitted>" "meinapetito"
CreateSecret "apetito:notification:api:ConnectionStrings:NServiceBusTransportConnectionString" "host=rabbitmq;username=guest;password=guest" "meinapetito"
CreateSecret "apetito:notification:api:ConnectionStrings:NServiceBusPersistenceConnectionString" "<value omitted>" "meinapetito"
CreateSecret "apetito:notification:api:SendGridSettings:ApiKey" "<Value omitted>" "meinapetito"


$prevPwd | Set-Location

