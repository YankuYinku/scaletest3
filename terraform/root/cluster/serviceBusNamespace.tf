module "serviceBusNamespace" {
  source = "../../modules/serviceBus"

  environment    = var.environment
  version_number = var.version_number
  location       = var.location

  oldInfrastructureResourceGroupName = var.oldInfrastructureResourceGroupName
  oldKeyVaultName                    = var.oldKeyVaultName
}
