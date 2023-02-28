resource "azurerm_subnet" "meinapetitonetworksubnet" {
  name                 = format("%s_%s_%s", var.environment, var.application_name, var.version_number)
  virtual_network_name = data.azurerm_virtual_network.meinapetito_vnet.name
  resource_group_name  = data.azurerm_resource_group.meinapetito_resourcegroup.name
  address_prefixes     = [var.clusterSubnetAddressSpace]
}

resource "azurerm_subnet_network_security_group_association" "meinapetitonetworksubnet_sec_group_assoc" {
  network_security_group_id = data.azurerm_network_security_group.meinapetitonetworksubnet_sec_group.id
  subnet_id                 = azurerm_subnet.meinapetitonetworksubnet.id
}

// add routing rule to ag rout table: from applicationgateway to kubernetes cluster - via firewall
resource "azurerm_route" "ApplicationGateway_To_KubernetesCluster" {
  name                   = format("To_KubernetesCluster_%s", var.version_number)
  resource_group_name    = data.azurerm_resource_group.meinapetito_resourcegroup.name
  route_table_name       = data.azurerm_route_table.routing_from_applicationGateway.name
  address_prefix         = var.clusterSubnetAddressSpace
  next_hop_type          = "VirtualAppliance"
  next_hop_in_ip_address = data.azurerm_firewall.infrastructure-firewallip.ip_configuration.0.private_ip_address
}

data "azurerm_subnet" "meinapetitonetworksubnet" {
  depends_on = [
    azurerm_subnet.meinapetitonetworksubnet,
    azurerm_kubernetes_cluster.meinapetitocluster
  ]

  name                 = format("%s_%s_%s", var.environment, var.application_name, var.version_number)
  virtual_network_name = data.azurerm_virtual_network.meinapetito_vnet.name
  resource_group_name  = data.azurerm_resource_group.meinapetito_resourcegroup.name
}

resource "azurerm_route" "KubernetesCluster_To_ApplicationGateway" {
  depends_on = [
    azurerm_subnet.meinapetitonetworksubnet,
    azurerm_kubernetes_cluster.meinapetitocluster
  ]

  name                   = "To_ApplicationGateway"
  resource_group_name    = data.azurerm_resource_group.mc_ressource_group.name
  route_table_name       = element(split("/", data.azurerm_subnet.meinapetitonetworksubnet.route_table_id), length(split("/", data.azurerm_subnet.meinapetitonetworksubnet.route_table_id)) - 1)
  address_prefix         = var.applicationGatewaySubnetAddressSpace
  next_hop_type          = "VirtualAppliance"
  next_hop_in_ip_address = data.azurerm_firewall.infrastructure-firewallip.ip_configuration.0.private_ip_address
}

resource "azurerm_route" "KubernetesCluster_To_Egress" {
  depends_on = [
    azurerm_subnet.meinapetitonetworksubnet,
    azurerm_kubernetes_cluster.meinapetitocluster
  ]
  
  name                   = "To_Egress"
  resource_group_name    = data.azurerm_resource_group.mc_ressource_group.name
  route_table_name       = element(split("/", data.azurerm_subnet.meinapetitonetworksubnet.route_table_id), length(split("/", data.azurerm_subnet.meinapetitonetworksubnet.route_table_id)) - 1)
  address_prefix         = "0.0.0.0/0"
  next_hop_type          = "VirtualAppliance"
  next_hop_in_ip_address = data.azurerm_firewall.infrastructure-firewallip.ip_configuration.0.private_ip_address
}

resource "azurerm_private_dns_a_record" "cluster_dns_record" {
  name = format("%s-%s", var.application_name, var.version_number)
  zone_name = data.azurerm_private_dns_zone.environemnt_apebs_de_dns_zone.name
  resource_group_name = data.azurerm_resource_group.infrastructure.name
  ttl = 60
  records = [ data.azurerm_public_ip.applicationGateway_public_ip.ip_address ]
}



