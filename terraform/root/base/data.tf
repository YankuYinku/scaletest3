data "azurerm_resource_group" "oldinfrastructureresourcegroup" {
  name = var.oldInfrastructureResourceGroupName
}

data "azurerm_key_vault" "oldkeyvault" {
  name                = var.oldKeyVaultName
  resource_group_name = data.azurerm_resource_group.oldinfrastructureresourcegroup.name
}

data "azurerm_resource_group" "infrastructureresourcegroup" {
  name = "${var.environment}_infrastructure"
}

data "azurerm_virtual_network" "hubvnet" {
  name                = "${var.environment}_infrastructure"
  resource_group_name = data.azurerm_resource_group.infrastructureresourcegroup.name
}

data "azurerm_firewall" "infrastructurefirewall"{
  name = "${var.environment}_infrastructure_firewall"
  resource_group_name = data.azurerm_resource_group.infrastructureresourcegroup.name
}