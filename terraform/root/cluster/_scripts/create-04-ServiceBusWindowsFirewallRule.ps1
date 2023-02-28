param (
    [Parameter(Mandatory)] [string] $Environment,
    [Parameter(Mandatory)] [string] $Version
)

# Prerequisite: Install extension azure-firewall
az config set extension.use_dynamic_install=yes_without_prompt

. .\helper.ps1

$MeinApetitoClusterSubnetAddressSpace = GetAddressPrefix $Environment $Version
$Application = "meinapetito"
$ServiceBusName = "${Environment}-${Application}-${Version}"
$FirewallResourceGroupName = "${Environment}_infrastructure"
$FirewallName = "${Environment}_infrastructure_firewall"
$FirewallRuleCollectionName = "${Environment}-${Application}"

$ServiceBusNamespaceDnsEntry = [System.Net.Dns]::GetHostEntry("${ServiceBusName}.servicebus.windows.net").AddressList[0].IPAddressToString
Write-Host $ServiceBusNamespaceDnsEntry


$numberOfExistingRules = $(az network firewall network-rule list -g $FirewallResourceGroupName -f $FirewallName --collection-name $FirewallRuleCollectionName --query="rules[?name=='$ServiceBusName-AMQP'] | length(@)")
if ($numberOfExistingRules -eq 0) 
{ 
  az network firewall network-rule create -g $FirewallResourceGroupName -f $FirewallName --collection-name $FirewallRuleCollectionName -n $ServiceBusName'-AMQP' --protocols 'TCP' --source-addresses $MeinApetitoClusterSubnetAddressSpace --destination-addresses $ServiceBusNamespaceDnsEntry --destination-ports 5671
} 
