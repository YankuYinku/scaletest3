param (
    [Parameter(Mandatory)] [string] $Environment,
    [Parameter(Mandatory)] [string] $Version
)

. .\_scripts\helper.ps1

$ClusterSubnetAddressSpace = GetFirstAvailableOrExistingSubnetRange $Environment $Version

terraform init -backend-config="$Environment/backend.tfvars" `
               -backend-config="key=$Environment.meinapetito.cluster.$Version.tfstate" `
               -reconfigure

# 
# To use this follow the instructions here: https://github.com/hieven/terraform-visual
#
terraform  plan -out="plan.out" `
                 -var-file="$Environment/env.tfvars" `
                 -var="clusterSubnetAddressSpace=$ClusterSubnetAddressSpace" `
                 -var="version_number=$Version"

terraform show -json plan.out > plan.json   # Read plan file and output it in JSON format
terraform-visual --plan plan.json
./terraform-visual-report/index.html


terraform apply -var-file="$Environment/env.tfvars" `
                -var="clusterSubnetAddressSpace=$ClusterSubnetAddressSpace" `
                -var="version_number=$Version"