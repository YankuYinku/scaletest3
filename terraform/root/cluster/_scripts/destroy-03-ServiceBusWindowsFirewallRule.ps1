param (
    [Parameter(Mandatory)] [string] $Environment,
    [Parameter(Mandatory)] [string] $Version
)

$Application = "meinapetito"
$ServiceBusName = "${Environment}-${Application}-${Version}"
$FirewallResourceGroupName = "${Environment}_infrastructure"
$FirewallName = "${Environment}_infrastructure_firewall"
$FirewallRuleCollectionName = "${Environment}-${Application}"

# Prerequisite: Install extension azure-firewall
az config set extension.use_dynamic_install=yes_without_prompt


$numberOfExistingRules = $(az network firewall network-rule list -g $FirewallResourceGroupName -f $FirewallName --collection-name $FirewallRuleCollectionName --query="rules[?name=='$ServiceBusName-AMQP'] | length(@)")
if ($numberOfExistingRules -ne 0) 
{ 
    az network firewall network-rule delete -g $FirewallResourceGroupName -f $FirewallName --collection-name $FirewallRuleCollectionName -n $ServiceBusName'-AMQP' 
} 
