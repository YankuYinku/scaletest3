oldInfrastructureResourceGroupName          = "production_infrastructure"
oldKeyVaultName                             = "apetitoKeyVault"
environment                                 = "prod"
applicationgatewaySubnetAddressSpace        = "10.42.12.0/26"
infrastructureSubnetAddressSpace            = "10.42.0.0/21"
sharedApplicationGatewaysSubnetAddressSpace = "10.42.18.0/27"
certificates                                = ["apetito-de-wildcard-ssl-certificate", "wildcard-prod-apebs-de", "apetito-microsites-ssl-certificate"]

applications = [
  {
    name                 = "meinapetito"
    host_names           = ["mein.apetito.de"]
    backend_target_type  = "ip"
    backend_target       = "10.42.14.13"
    ssl_termination      = true
    request_timeout      = 30
    health_probe_path    = "/"
    ssl_certificate_name = "apetito-de-wildcard-ssl-certificate"
  },

  {
    name                 = "meinapetito-domainredirections-de"
    host_names           = ["meinapetito.de", "www.meinapetito.de", "mein-apetito.de", "www.mein-apetito.de"]
    backend_target_type  = "redirect"
    backend_target       = "meinapetito" // refers to the application with name "meinapetito" (see above)
    ssl_termination      = false
    request_timeout      = 30
    health_probe_path    = "/"
    ssl_certificate_name = "apetito-microsites-ssl-certificate"
  },

    {
    name                 = "meinapetito-domainredirections-at"
    host_names           = ["meinapetito.at", "www.meinapetito.at"]
    backend_target_type  = "redirect"
    backend_target       = "meinapetito" // refers to the application with name "meinapetito" (see above)
    ssl_termination      = false
    request_timeout      = 30
    health_probe_path    = "/"
    ssl_certificate_name = "apetito-microsites-ssl-certificate"
  },

  {
    name                 = "meinapetito-domainredirections-nl"
    host_names           = ["mijnapetito.nl", "www.mijnapetito.nl"]
    backend_target_type  = "redirect"
    backend_target       = "meinapetito" // refers to the application with name "meinapetito" (see above)
    ssl_termination      = false
    request_timeout      = 30
    health_probe_path    = "/"
    ssl_certificate_name = "apetito-microsites-ssl-certificate"
  },

  {
    name                 = "menueauswahl"
    host_names           = ["menueauswahl.apetito.de"]
    backend_target_type  = "fqdn"
    backend_target       = "shared-ibssc.prod.apebs.de"
    ssl_termination      = false
    request_timeout      = 60
    health_probe_path    = "/"
    ssl_certificate_name = "apetito-de-wildcard-ssl-certificate"
  },
  {
    name                 = "speiseplanung"
    host_names           = ["speiseplanung.apetito.de"]
    backend_target_type  = "fqdn"
    backend_target       = "shared-menuplanner.prod.apebs.de"
    ssl_termination      = false
    request_timeout      = 30
    health_probe_path    = "/"
    ssl_certificate_name = "apetito-de-wildcard-ssl-certificate"
  },
  {
    name                 = "speiseplandruck"
    host_names           = ["speiseplandruck.apetito.de"]
    backend_target_type  = "fqdn"
    backend_target       = "shared-printtool.prod.apebs.de"
    ssl_termination      = false
    request_timeout      = 30
    health_probe_path    = "/health"
    ssl_certificate_name = "apetito-de-wildcard-ssl-certificate"
  }
]
