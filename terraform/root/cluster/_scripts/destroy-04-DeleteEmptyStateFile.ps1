param (
    [Parameter(Mandatory)] [string] $Environment,
    [Parameter(Mandatory)] [string] $Version
)

$TerraformStateStorageAccountName = "ap${Environment}infrastructure"
$TerraformStateBlobContainerName = "terraformstate"
$TerraformStateKey = "${Environment}.meinapetito.cluster.${Version}.tfstate"

az storage blob delete --account-name $TerraformStateStorageAccountName --container-name $TerraformStateBlobContainerName --name $TerraformStateKey