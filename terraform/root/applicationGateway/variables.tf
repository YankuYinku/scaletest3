variable "environment" {
  type = string
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

variable "oldInfrastructureResourceGroupName" {
  type = string
}

variable "oldKeyVaultName" {
  type = string
}

variable "applicationgatewaySubnetAddressSpace" {
  type = string
}

variable "infrastructureSubnetAddressSpace" {
  type = string
}

variable "sharedApplicationGatewaysSubnetAddressSpace" {
  type = string
}

variable "certificates" {
  type = list(string)
}

variable "applications" {
  type = list(object({
    name                  = string
    host_names        = list(string)

    backend_target_type = string // "ip" or "fqdn"
    backend_target      = string

    ssl_termination      = bool
    request_timeout      = number
    health_probe_path    = string
    ssl_certificate_name = string
  }))
}
