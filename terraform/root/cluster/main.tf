terraform {
  backend "azurerm" {
  }

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = ">=3.0.0"
    }
  }
}

provider "azurerm" {
  features {}
}

locals {
  common_tags = {
    environment = var.environment
    application = var.application_name
    company     = var.company_name
    department  = var.department_name
  }
}

resource "azurerm_kubernetes_cluster" "meinapetitocluster" {
  resource_group_name = data.azurerm_resource_group.meinapetito_resourcegroup.name
  location            = data.azurerm_resource_group.meinapetito_resourcegroup.location

  name       = format("%s-%s-%s", var.environment, var.application_name, var.version_number)
  dns_prefix = format("%s-%s-%s", var.environment, var.application_name, var.version_number)

  role_based_access_control_enabled = true

  # Not yet supported in azurerm 2.78, and causing an error in 2.79
  # automatic_channel_upgrade = "none"

  default_node_pool {
    name                = "default"
    node_count          = var.clusterNodeCount
    enable_auto_scaling = var.environment == "dev" ? false : true
    min_count           = var.environment == "dev" ? null : var.clusterNodeCount
    max_count           = var.environment == "dev" ? null : var.clusterNodeCount * 2
    vm_size             = var.clusterVmSize
    vnet_subnet_id      = azurerm_subnet.meinapetitonetworksubnet.id
  }

  identity {
    type = "SystemAssigned"
  }

  oms_agent {
    log_analytics_workspace_id = azurerm_log_analytics_workspace.meinapetitoLogAnalyticsWorkspace.id
  }


  tags = local.common_tags
}

resource "azurerm_role_assignment" "acrpull_role" {
  scope                            = data.azurerm_container_registry.apetitoebusinesssolutions.id
  role_definition_name             = "AcrPull"
  principal_id                     = azurerm_kubernetes_cluster.meinapetitocluster.kubelet_identity[0].object_id
  skip_service_principal_aad_check = true
}


resource "azurerm_role_assignment" "mc_ressource_group_networkcontributor_role" {
  depends_on = [
    azurerm_kubernetes_cluster.meinapetitocluster
  ]

  scope                            = data.azurerm_resource_group.mc_ressource_group.id
  role_definition_name             = "Network Contributor"
  principal_id                     = azurerm_kubernetes_cluster.meinapetitocluster.identity[0].principal_id
  skip_service_principal_aad_check = true
}

resource "azurerm_role_assignment" "ressource_group_networkcontributor_role" {
  depends_on = [
    azurerm_kubernetes_cluster.meinapetitocluster
  ]

  scope                            = data.azurerm_resource_group.meinapetito_resourcegroup.id
  role_definition_name             = "Network Contributor"
  principal_id                     = azurerm_kubernetes_cluster.meinapetitocluster.identity[0].principal_id
  skip_service_principal_aad_check = true
}

resource "azurerm_role_assignment" "ressource_group_reader_role" {
  depends_on = [
    azurerm_kubernetes_cluster.meinapetitocluster
  ]

  scope                            = data.azurerm_resource_group.meinapetito_resourcegroup.id
  role_definition_name             = "Reader"
  principal_id                     = azurerm_kubernetes_cluster.meinapetitocluster.identity[0].principal_id
  skip_service_principal_aad_check = true
}

resource "azurerm_role_assignment" "aks_network_Network_Contributor_role" {
  depends_on = [
    azurerm_kubernetes_cluster.meinapetitocluster
  ]

  scope                            = data.azurerm_virtual_network.meinapetito_vnet.id
  role_definition_name             = "Network Contributor"
  principal_id                     = azurerm_kubernetes_cluster.meinapetitocluster.identity[0].principal_id
  skip_service_principal_aad_check = true
}