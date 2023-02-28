param (
    [Parameter(Mandatory)] [string] $Environment,
    [Parameter(Mandatory)] [int] $Version
)

. .\helper.ps1
$KeyVaultName = GetKeyVaultNameByEnvironment $Environment

$PortalManagementApplicationClientSecret = $( az keyvault secret show --vault-name $KeyVaultName --name "apetito-AzureADB2C-AppRegistration-PortalManagement-ClientSecret" --output tsv --query "value" )
$PortalManagementApplicationClientId = $( az keyvault secret show --vault-name $KeyVaultName --name "apetito-AzureADB2C-AppRegistration-PortalManagement-ClientId" --output tsv --query "value" )
$PortalApiApplicationObjectId = $( az keyvault secret show --vault-name $KeyVaultName --name "apetito-AzureADB2C-AppRegistration-PortalApi-ObjectId" --output tsv --query "value" )
$PortalRootApplicationObjectId = $( az keyvault secret show --vault-name $KeyVaultName --name "apetito-AzureADB2C-AppRegistration-PortalRoot-ObjectId" --output tsv --query "value" )
$AzureADB2CTenantId = $( az keyvault secret show --vault-name $KeyVaultName --name "apetito-AzureADB2C-TenantId" --output tsv --query "value" )

# Get MS Graph API access token
$tokenResponse = Invoke-WebRequest https://login.microsoftonline.com/$AzureADB2CTenantId/oauth2/v2.0/token `
    -Method 'POST' `
    -Body @{ 'client_id' = $PortalManagementApplicationClientId; 'scope' = 'https://graph.microsoft.com/.default'; 'client_secret' = $PortalManagementApplicationClientSecret; 'grant_type' = 'client_credentials' }
$accessToken = ($tokenResponse.Content | ConvertFrom-Json).access_token

# Get Poral-Api application object
$portalApiApplicationResponse = Invoke-WebRequest https://graph.microsoft.com/v1.0/applications/$PortalApiApplicationObjectId `
    -Method 'GET' `
    -Headers @{ 'Authorization' = 'Bearer ' + $accessToken }
$portalApiApplication = $portalApiApplicationResponse | ConvertFrom-Json

# Add redirect Uri for portal api
$portalApiRedirectUri = "https://meinapetito-$Version.$Environment.apebs.de/api/swagger/oauth2-redirect.html"

if (!$portalApiApplication.web.redirectUris.contains($portalApiRedirectUri))
{
    $portalApiApplication.web.redirectUris += $portalApiRedirectUri
}

$webPropertyJson = $portalApiApplication.web | ConvertTo-Json
$patchBody = '{ "web": ' + $webPropertyJson + '}'

Invoke-WebRequest https://graph.microsoft.com/v1.0/applications/$PortalApiApplicationObjectId `
    -Method 'PATCH' `
    -ContentType 'application/json; charset=utf-8' `
    -Headers @{ 'Authorization' = 'Bearer ' + $accessToken } `
    -Body $patchBody


# Get Poral-root application object
$portalRootApplicationResponse = Invoke-WebRequest https://graph.microsoft.com/v1.0/applications/$PortalRootApplicationObjectId `
    -Method 'GET' `
    -Headers @{ 'Authorization' = 'Bearer ' + $accessToken }
$portalRootApplication = $portalRootApplicationResponse | ConvertFrom-Json

# Add redirect Uris for portal api
$portalRootRedirectRootUri = "https://meinapetito-$Version.$Environment.apebs.de"
if (!$portalRootApplication.spa.redirectUris.contains($portalRootRedirectRootUri))
{
    $portalRootApplication.spa.redirectUris += $portalRootRedirectRootUri
}

$portalRootRedirectSignupUri = "https://meinapetito-$Version.$Environment.apebs.de/signin"
if (!$portalRootApplication.spa.redirectUris.contains($portalRootRedirectSignupUri))
{
    $portalRootApplication.spa.redirectUris += $portalRootRedirectSignupUri
}

$spaPropertyJson = $portalRootApplication.spa | ConvertTo-Json
$patchBody = '{ "spa": ' + $spaPropertyJson + '}'

Invoke-WebRequest https://graph.microsoft.com/v1.0/applications/$PortalRootApplicationObjectId `
    -Method 'PATCH' `
    -ContentType 'application/json; charset=utf-8' `
    -Headers @{ 'Authorization' = 'Bearer ' + $accessToken } `
    -Body $patchBody

Write-Host "#####################################################################################################" -ForegroundColor green
Write-Host
Write-Host "Your cluster url: https://meinapetito-$Version.$Environment.apebs.de" -ForegroundColor green
Write-Host
Write-Host "#####################################################################################################" -ForegroundColor green
