$prevPwd = $PWD; Set-Location -ErrorAction Stop -LiteralPath $PSScriptRoot

. "./base/create-appSettings.ps1"



## SETUP ENVIRONMENT
# DevKeyVault Dev
$APPCONFIG_NAME = "apetitoAppSettings-development";
$KEYVAULT_NAME = "apetitoKeyVault-dev";

# DevKeyVault Stage
# $APPCONFIG_NAME = "apetitoAppSettings-staging"; 
# $KEYVAULT_NAME = "apetitoKeyVault-staging";

# DevKeyVault Prod
# $APPCONFIG_NAME = "apetitoAppSettings";
# $KEYVAULT_NAME = "apetitoKeyVault";

## SETUP VALUES
# WARNING: All Actions will check if the entry already exists, if so, it will not be created again / overwritten.
# use the "Forec..." version to force the creation of the entry / overwrite.

CreateSetting "<keyprefix:Path>" "<Value Omitted>"
ForceCreateSetting "<keyprefix:Path>" "<Value Omitted>"

CreateSecretWithoutAppSetting "<keyprefix:Path>" "<Value Omitted>"
ForceCreateSecretWithoutAppSetting "<keyprefix:Path>" "<Value Omitted>"

CreateSettingWithExistingKeyVaultEntry "<keyprefix:Path>" "<Key Vault Reference>"
ForceCreateSettingWithExistingKeyVaultEntry "<keyprefix:Path>" "<Key Vault Reference>"

CreateSecret "<keyprefix:Path>" "<Value Omitted>"
ForceCreateSecret "<keyprefix:Path>" "<Value Omitted>"

$prevPwd | Set-Location

