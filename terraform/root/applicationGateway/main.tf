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

module "applicationGateway" {
  source = "../../modules/applicationGateway"

  environment                                 = var.environment
  oldInfrastructureResourceGroupName          = var.oldInfrastructureResourceGroupName
  oldKeyVaultName                             = var.oldKeyVaultName
  applicationgatewaySubnetAddressSpace        = var.applicationgatewaySubnetAddressSpace
  infrastructureSubnetAddressSpace            = var.infrastructureSubnetAddressSpace
  sharedApplicationGatewaysSubnetAddressSpace = var.sharedApplicationGatewaysSubnetAddressSpace
  certificates                                = var.certificates
  applications                                = var.applications
}
