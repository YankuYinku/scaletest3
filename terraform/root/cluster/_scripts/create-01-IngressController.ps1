param (
    [Parameter(Mandatory)] [string] $Environment,
    [Parameter(Mandatory)] [string] $Version
)

. .\helper.ps1

$LoadBalancerIp = GetLoadBalancerIp $Environment $Version
Write-Host $LoadBalancerIp
$Application = "meinapetito"
$RESSOURCE_GROUP = $Environment + "_" + $Application
$CLUSTER_NAME = "$Environment-$Application-$Version"
$REPLICA_COUNT = 3
$LOADBALANCER_RESOURCE_GROUP = "MC_" + $RESSOURCE_GROUP + "_" + $CLUSTER_NAME + "_westeurope"

az aks get-credentials -g $RESSOURCE_GROUP -n $CLUSTER_NAME --overwrite-existing

# This creates the namespace if it does not exists and wont complain otherwise (https://stackoverflow.com/questions/63135361/how-to-create-kubernetes-namespace-if-it-does-not-exist)
kubectl create namespace ingress-nginx --dry-run=client -o yaml | kubectl apply -f -

helm repo add ingress-nginx https://kubernetes.github.io/ingress-nginx
helm repo update

helm upgrade ingress-nginx ingress-nginx/ingress-nginx --install `
    --namespace ingress-nginx `
    --set controller.replicaCount=$REPLICA_COUNT `
    --set controller.service.externalTrafficPolicy=Local `
    --set controller.nodeSelector."kubernetes\.io/os"=linux `
    --set defaultBackend.nodeSelector."kubernetes\.io/os"=linux `
    --set controller.admissionWebhooks.patch.nodeSelector."kubernetes\.io/os"=linux `
    --set controller.service.annotations."service\.beta\.kubernetes\.io/azure-load-balancer-resource-group"="$LOADBALANCER_RESOURCE_GROUP" `
    --set controller.service.annotations."service\.beta\.kubernetes\.io/azure-load-balancer-internal"="true" `
    --set controller.service.loadBalancerIP=$LoadBalancerIp

