variable "environment" {
  type = string
}

variable "version_number" {
  type = string
  description = "The feature number / build number / version number that is used as an index to distinguish between mutliple clusters in the same environment."
}

variable "application_name" {
  type    = string
  default = "meinapetito"
}

variable "company_name" {
  type    = string
  default = "apetito AG"
}

variable "department_name" {
  type    = string
  default = "eBusiness Solutions"
}

variable "location" {
  type    = string
  default = "West Europe"
}

variable "applicationGatewaySubnetAddressSpace" {
  type    = string
  default = "West Europe"
}


variable "clusterSubnetAddressSpace" {
  type    = string
  default = "West Europe"
}

variable "clusterNodeCount" {
  description = "Count of the nodes inside this cluster"
  type        = string
}

variable "clusterVmSize" {
  description = "Pricing Tier for the VM of the cluster"
  type        = string
}

variable "oldInfrastructureResourceGroupName" {
  type = string
}

variable "oldKeyVaultName" {
  type = string
}
