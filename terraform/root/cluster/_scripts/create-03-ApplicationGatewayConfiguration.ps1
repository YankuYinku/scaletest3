param (
    [Parameter(Mandatory)] [string] $Environment,
    [Parameter(Mandatory)] [string] $Version
)

. .\helper.ps1

$LoadBalancerIp = GetLoadBalancerIp $Environment $Version
$Application = "meinapetito"
$RESSOURCE_GROUP = $Environment + "_" + $Application
$GATEWAY_NAME = $Environment + "_" + $Application

# Define names
$CLUSTER_HOST_NAME = "$Application-$Version.$Environment.apebs.de"

$BACKEND_ADDRESS_POOL_NAME = "$RESSOURCE_GROUP-$Application-$Version-beap"
$BACKEND_HTTP_SETTINGS_NAME = "$RESSOURCE_GROUP-$Application-$Version-be-htst"
$HEALTH_PROBE_NAME = "$RESSOURCE_GROUP-$Application-$Version-probe"
$LISTENER_HTTP_NAME = "$RESSOURCE_GROUP-$Application-$Version-httplstn-http"
$LISTENER_HTTPS_NAME = "$RESSOURCE_GROUP-$Application-$Version-httplstn-https"
$ROUTING_RULE_HTTP_NAME = "$RESSOURCE_GROUP-$Application-$Version-rqrt"
$ROUTING_RULE_HTTPS_NAME = "$RESSOURCE_GROUP-$Application-$Version-rqrt-https"
$REDIRECT_CONFIG_NAME = "$RESSOURCE_GROUP-$Application-$Version-rqrt-http-redirect"

$FRONTENT_IP_CONFIGURATION_NAME = $Environment + "_meinapetito-feip"
$FRONTEND_PORT_HTTP_NAME = $Environment + "_meinapetito-feport"
$FRONTENT_PORT_HTTPS_NAME = $Environment + "_meinapetito-feport-https"
$SSL_CERTIFICATE_NAME = "wildcard-$Environment-apebs-de"
$SSL_PROFILE_NAME = $Environment + "_meinapetito_ssl_profile"

# Create Backend pool + backend settings with target ip of private load balancer
az network application-gateway address-pool create -g $RESSOURCE_GROUP --gateway-name $GATEWAY_NAME -n $BACKEND_ADDRESS_POOL_NAME --servers $LoadBalancerIp
az network application-gateway probe create -g $RESSOURCE_GROUP --gateway-name $GATEWAY_NAME -n $HEALTH_PROBE_NAME --host $CLUSTER_HOST_NAME --protocol Http --path /
az network application-gateway http-settings create -g $RESSOURCE_GROUP --gateway-name $GATEWAY_NAME -n $BACKEND_HTTP_SETTINGS_NAME --port 80 --protocol Http --cookie-based-affinity Disabled --timeout 30 --connection-draining-timeout 60 --probe $HEALTH_PROBE_NAME

# Listener 
$SSL_PROFILE_ID = $(az network application-gateway ssl-profile show -g $RESSOURCE_GROUP --gateway-name $GATEWAY_NAME --name $SSL_PROFILE_NAME --out tsv --query "id")
az network application-gateway http-listener create -g $RESSOURCE_GROUP --gateway-name $GATEWAY_NAME -n $LISTENER_HTTPS_NAME --frontend-port $FRONTENT_PORT_HTTPS_NAME --frontend-ip $FRONTENT_IP_CONFIGURATION_NAME --host-name $CLUSTER_HOST_NAME --ssl-cert $SSL_CERTIFICATE_NAME --ssl-profile-id $SSL_PROFILE_ID
az network application-gateway http-listener create -g $RESSOURCE_GROUP --gateway-name $GATEWAY_NAME -n $LISTENER_HTTP_NAME --frontend-port $FRONTEND_PORT_HTTP_NAME --frontend-ip $FRONTENT_IP_CONFIGURATION_NAME --host-name $CLUSTER_HOST_NAME

# Rules
## Actual routing to backend pool
$highestPriority = [int]$(az network application-gateway rule list -g $RESSOURCE_GROUP --gateway-name $GATEWAY_NAME --query "[].priority | max(@)") + 50
az network application-gateway rule create -g $RESSOURCE_GROUP --gateway-name $GATEWAY_NAME -n $ROUTING_RULE_HTTPS_NAME --http-listener $LISTENER_HTTPS_NAME --address-pool $BACKEND_ADDRESS_POOL_NAME --http-settings $BACKEND_HTTP_SETTINGS_NAME --priority $highestPriority

## Redrect http -> https
$highestPriority = [int]$(az network application-gateway rule list -g $RESSOURCE_GROUP --gateway-name $GATEWAY_NAME --query "[].priority | max(@)") + 50
az network application-gateway redirect-config create -g $RESSOURCE_GROUP --gateway-name $GATEWAY_NAME -n $REDIRECT_CONFIG_NAME -t Permanent --target-listener $LISTENER_HTTPS_NAME --include-path true --include-query-string true
az network application-gateway rule create -g $RESSOURCE_GROUP --gateway-name $GATEWAY_NAME -n $ROUTING_RULE_HTTP_NAME --http-listener $LISTENER_HTTP_NAME --redirect-config $REDIRECT_CONFIG_NAME --priority $highestPriority
