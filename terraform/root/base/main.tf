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

resource "azurerm_resource_group" "resource_group" {
  name     = format("%s_%s", var.environment, var.application_name)
  location = var.location

  tags = local.common_tags
}

resource "azurerm_storage_account" "meinapetito_storage_account" {
  account_replication_type = "LRS"
  account_tier             = "Standard"
  location                 = var.location
  name                     = format("%s%s%s", "ap", var.environment, var.application_name)
  resource_group_name      = azurerm_resource_group.resource_group.name

  tags = local.common_tags
}

