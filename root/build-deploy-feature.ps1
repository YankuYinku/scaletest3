param (
    [Parameter(Position = 0)]
    [string] $PAT 
)

$prevPwd = $PWD; Set-Location -ErrorAction Stop -LiteralPath $PSScriptRoot

./build.ps1 $PAT
./kubernetes/deploy-feature.ps1

$prevPwd | Set-Location
