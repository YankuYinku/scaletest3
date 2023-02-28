resource "azurerm_log_analytics_workspace" "meinapetitoLogAnalyticsWorkspace" {
  # The WorkSpace name has to be unique across the whole of azure, not just the current subscription/tenant.
  name                = format("%s-%s-%s", var.environment, var.application_name, var.version_number)
  location            = data.azurerm_resource_group.meinapetito_resourcegroup.location
  resource_group_name = data.azurerm_resource_group.meinapetito_resourcegroup.name
  sku                 = "PerGB2018"

  tags = local.common_tags
}

resource "azurerm_log_analytics_solution" "meinapetitoLogAnalyticsSolution" {
  solution_name         = "ContainerInsights" # This name must not be changed, otherwise you recieve an error (see https://github.com/hashicorp/terraform-provider-azurerm/issues/1775)
  location              = azurerm_log_analytics_workspace.meinapetitoLogAnalyticsWorkspace.location
  resource_group_name   = data.azurerm_resource_group.meinapetito_resourcegroup.name
  workspace_resource_id = azurerm_log_analytics_workspace.meinapetitoLogAnalyticsWorkspace.id
  workspace_name        = azurerm_log_analytics_workspace.meinapetitoLogAnalyticsWorkspace.name

  plan {
    publisher = "Microsoft"
    product   = "OMSGallery/ContainerInsights"
  }

  tags = local.common_tags
}
