param (
    [Parameter(Position = 0)]
    [string] $PAT 
)

$prevPwd = $PWD; Set-Location -ErrorAction Stop -LiteralPath $PSScriptRoot

./src/apetito.meinapetito.Webhooks.ArticleChanges.Api/build.ps1 $PAT
./deploy-feature.ps1

$prevPwd | Set-Location
