param (
    [Parameter(Mandatory)] [string] $Environment,
    [Parameter(Mandatory)] [string] $Version
)

$Application = "meinapetito"
$RESSOURCE_GROUP = $Environment + "_" + $Application
$GATEWAY_NAME = $Environment + "_" + $Application

$BACKEND_ADDRESS_POOL_NAME = "$RESSOURCE_GROUP-$Application-$Version-beap"
$BACKEND_HTTP_SETTINGS_NAME = "$RESSOURCE_GROUP-$Application-$Version-be-htst"
$HEALTH_PROBE_NAME = "$RESSOURCE_GROUP-$Application-$Version-probe"
$LISTENER_HTTP_NAME = "$RESSOURCE_GROUP-$Application-$Version-httplstn-http"
$LISTENER_HTTPS_NAME = "$RESSOURCE_GROUP-$Application-$Version-httplstn-https"
$ROUTING_RULE_HTTP_NAME = "$RESSOURCE_GROUP-$Application-$Version-rqrt"
$ROUTING_RULE_HTTPS_NAME = "$RESSOURCE_GROUP-$Application-$Version-rqrt-https"
$REDIRECT_CONFIG_NAME = "$RESSOURCE_GROUP-$Application-$Version-rqrt-http-redirect"

# Rules
## Actual routing to backend pool
az network application-gateway rule delete -g $RESSOURCE_GROUP --gateway-name $GATEWAY_NAME -n $ROUTING_RULE_HTTPS_NAME 

## Redrect http -> https
az network application-gateway rule delete -g $RESSOURCE_GROUP --gateway-name $GATEWAY_NAME -n $ROUTING_RULE_HTTP_NAME 
az network application-gateway redirect-config delete -g $RESSOURCE_GROUP --gateway-name $GATEWAY_NAME -n $REDIRECT_CONFIG_NAME 

# delete Backend pool + backend settings + probe
az network application-gateway address-pool delete -g $RESSOURCE_GROUP --gateway-name $GATEWAY_NAME -n $BACKEND_ADDRESS_POOL_NAME 
az network application-gateway http-settings delete -g $RESSOURCE_GROUP --gateway-name $GATEWAY_NAME -n $BACKEND_HTTP_SETTINGS_NAME 
az network application-gateway probe delete -g $RESSOURCE_GROUP --gateway-name $GATEWAY_NAME -n $HEALTH_PROBE_NAME 

# Listener 
az network application-gateway http-listener delete -g $RESSOURCE_GROUP --gateway-name $GATEWAY_NAME -n $LISTENER_HTTPS_NAME 
az network application-gateway http-listener delete -g $RESSOURCE_GROUP --gateway-name $GATEWAY_NAME -n $LISTENER_HTTP_NAME 


