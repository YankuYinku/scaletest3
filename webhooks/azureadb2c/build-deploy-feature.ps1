param (
    [Parameter(Position = 0)]
    [string] $PAT 
)

$prevPwd = $PWD; Set-Location -ErrorAction Stop -LiteralPath $PSScriptRoot

./src/apetito.meinapetito.Webhooks.AzureADB2C.Api/build.ps1 $PAT
./deploy-feature.ps1

$prevPwd | Set-Location
