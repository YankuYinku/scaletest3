oldInfrastructureResourceGroupName          = "development_infrastructure"
oldKeyVaultName                             = "apetitoKeyVault-dev"
environment                                 = "dev"
applicationgatewaySubnetAddressSpace        = "10.40.12.0/26"
infrastructureSubnetAddressSpace            = "10.40.0.0/21"
sharedApplicationGatewaysSubnetAddressSpace = "10.40.18.0/27"
certificates                                = ["wildcard-dev-apebs-de"]

applications = [
  {
    name                 = "menueauswahl"
    host_names           = ["menueauswahl.apetito.de"]
    backend_target_type  = "fqdn"
    backend_target       = "shared-ibssc.dev.apebs.de"
    ssl_termination      = false
    request_timeout      = 60
    health_probe_path    = "/"
    ssl_certificate_name = "wildcard-dev-apebs-de"
  },
  {
    name                 = "speiseplanung"
    host_names           = ["speiseplanung.apetito.de"]
    backend_target_type  = "fqdn"
    backend_target       = "shared-menuplanner.dev.apebs.de"
    ssl_termination      = false
    request_timeout      = 30
    health_probe_path    = "/"
    ssl_certificate_name = "wildcard-dev-apebs-de"
  },
  {
    name                 = "speiseplandruck"
    host_names           = ["speiseplandruck.apetito.de"]
    backend_target_type  = "fqdn"
    backend_target       = "shared-printtool.dev.apebs.de"
    ssl_termination      = false
    request_timeout      = 30
    health_probe_path    = "/health"
    ssl_certificate_name = "wildcard-dev-apebs-de"
  }
]
