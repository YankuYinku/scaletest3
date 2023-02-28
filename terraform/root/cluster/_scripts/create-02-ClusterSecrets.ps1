param (
    [Parameter(Mandatory)] [string] $Environment,
    [Parameter(Mandatory)] [string] $Version
)

. .\helper.ps1

$Application = "meinapetito"
$RESSOURCE_GROUP = $Environment + "_" + $Application
$CLUSTER_NAME = "$Environment-$Application-$Version"
$KEY_VAULT_NAME = GetKeyVaultNameByEnvironment $Environment

az aks get-credentials -g $RESSOURCE_GROUP -n $CLUSTER_NAME --overwrite-existing

$MeinApetitoServicePrincipalClientId = $(az keyvault secret show --vault-name $KEY_VAULT_NAME --name "apetito-meinapetito-serviceprincipal-clientid" --query "value")
$MeinApetitoServicePrincipalClientSecret = $(az keyvault secret show --vault-name $KEY_VAULT_NAME --name "apetito-meinapetito-serviceprincipal-clientsecret" --query "value")
$MeinApetitoAppSettingsUri = $(az keyvault secret show --vault-name $KEY_VAULT_NAME --name "apetito-meinapetito-appsettings-uri" --query "value")

kubectl create secret generic appsetting-sserviceprincipal `
   --from-literal=AZURE_CLIENT_ID=$MeinApetitoServicePrincipalClientId `
   --from-literal=AZURE_CLIENT_SECRET=$MeinApetitoServicePrincipalClientSecret `
   --from-literal=AZURE_APP_SETTINGS_URI=$MeinApetitoAppSettingsUri `
   --dry-run=client -o yaml | kubectl apply -f -

$NServiceBusTransportConnectionString = $(az keyvault secret show --vault-name $KEY_VAULT_NAME --name "apetito-meinapetito-azureservicebusnamespace-primaryconnectionstring-$Version" --query "value")

kubectl create secret generic nservicebus-transport-connectionstring `
   --from-literal=NServiceBusTransportConnectionString=$NServiceBusTransportConnectionString `
   --dry-run=client -o yaml | kubectl apply -f -
   