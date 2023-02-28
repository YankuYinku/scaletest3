oldInfrastructureResourceGroupName          = "staging_infrastructure"
oldKeyVaultName                             = "apetitoKeyVault-staging"
environment                                 = "stage"
applicationgatewaySubnetAddressSpace        = "10.41.12.0/26"
infrastructureSubnetAddressSpace            = "10.41.0.0/21"
sharedApplicationGatewaysSubnetAddressSpace = "10.41.18.0/27"
certificates                                = ["wildcard-stage-apebs-de", "wildcard-apebs-de"]

applications = [
  {
    name                 = "meinapetito"
    host_names           = ["stage-meinapetito.apebs.de"]
    backend_target_type  = "ip"
    backend_target       = "10.41.14.29"
    ssl_termination      = true
    request_timeout      = 30
    health_probe_path    = "/"
    ssl_certificate_name = "wildcard-apebs-de"
  },
  {
    name                 = "meinapetito-stage"
    host_names           = ["meinapetito.stage.apebs.de"]
    backend_target_type  = "ip"
    backend_target       = "10.41.14.29"
    ssl_termination      = true
    request_timeout      = 30
    health_probe_path    = "/"
    ssl_certificate_name = "wildcard-stage-apebs-de"
  },
  {
    name                 = "menueauswahl"
    host_names           = ["menueauswahl.apetito.de"]
    backend_target_type  = "fqdn"
    backend_target       = "shared-ibssc.stage.apebs.de"
    ssl_termination      = false
    request_timeout      = 60
    health_probe_path    = "/"
    ssl_certificate_name = "wildcard-stage-apebs-de"
  },
  {
    name                 = "speiseplanung"
    host_names           = ["speiseplanung.apetito.de"]
    backend_target_type  = "fqdn"
    backend_target       = "shared-menuplanner.stage.apebs.de"
    ssl_termination      = false
    request_timeout      = 30
    health_probe_path    = "/"
    ssl_certificate_name = "wildcard-stage-apebs-de"
  },
  {
    name                 = "speiseplandruck"
    host_names           = ["speiseplandruck.stage.apebs.de"]
    backend_target_type  = "fqdn"
    backend_target       = "shared-printtool.stage.apebs.de"
    ssl_termination      = false
    request_timeout      = 30
    health_probe_path    = "/health"
    ssl_certificate_name = "wildcard-stage-apebs-de"
  }
]
