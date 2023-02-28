data "azurerm_resource_group" "meinapetito_resourcegroup" {
  name = format("%s_%s", var.environment, var.application_name)
}

data "azurerm_virtual_network" "meinapetito_vnet" {
  name                = format("%s_%s", var.environment, var.application_name)
  resource_group_name = data.azurerm_resource_group.meinapetito_resourcegroup.name
}

data "azurerm_container_registry" "apetitoebusinesssolutions" {
  name                = "apetitoebusinesssolutions"
  resource_group_name = "production_infrastructure"
}

data "azurerm_resource_group" "mc_ressource_group" {
  name = "MC_${data.azurerm_resource_group.meinapetito_resourcegroup.name}_${var.environment}-${var.application_name}-${var.version_number}_${data.azurerm_resource_group.meinapetito_resourcegroup.location}"
  depends_on = [
    azurerm_kubernetes_cluster.meinapetitocluster
  ]
}

data "azurerm_resource_group" "infrastructure" {
  name = "${var.environment}_infrastructure"
}

data "azurerm_firewall" "infrastructure-firewallip" {
  name                = "${var.environment}_infrastructure_firewall"
  resource_group_name = data.azurerm_resource_group.infrastructure.name
}

data "azurerm_private_dns_zone" "environemnt_apebs_de_dns_zone" {
  name = "${var.environment}.apebs.de"
  resource_group_name = data.azurerm_resource_group.infrastructure.name
}

data "azurerm_public_ip" "applicationGateway_public_ip" {
  name = format("%s_%s_%s", var.environment, var.application_name, "applicationgateway")
  resource_group_name = data.azurerm_resource_group.meinapetito_resourcegroup.name
}

data "azurerm_resource_group" "oldinfrastructureresourcegroup" {
  name = var.oldInfrastructureResourceGroupName
}

data "azurerm_key_vault" "oldkeyvault" {
  name                = var.oldKeyVaultName
  resource_group_name = data.azurerm_resource_group.oldinfrastructureresourcegroup.name
}

data "azurerm_route_table" "routing_from_applicationGateway" {
  name                = "${var.environment}_${var.application_name}_routing_applicationgateway"
  resource_group_name = data.azurerm_resource_group.meinapetito_resourcegroup.name
}

data "azurerm_network_security_group" "meinapetitonetworksubnet_sec_group" {
  name = format("%s_%s_clusters", var.environment, var.application_name)
  resource_group_name = data.azurerm_resource_group.meinapetito_resourcegroup.name
}