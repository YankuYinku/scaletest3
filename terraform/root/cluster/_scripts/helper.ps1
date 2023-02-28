function GetEnvironmentIpByte
{
    param ([Parameter(Mandatory)] [string] $Environment)
    switch ($Environment)
    {
        'dev' {
            $result = "40"
        }
        'stage' {
            $result = "41"
        }
        'prod' {
            $result = "42"
        }
    }

    $result
}

function GetKeyVaultNameByEnvironment
{
    param ([Parameter(Mandatory)] [string] $Environment)
    switch ($Environment)
    {
        'dev' {
            $result = "apetitoKeyVault-dev"
        }
        'stage' {
            $result = "apetitoKeyVault-staging"
        }
        'prod' {
            $result = "apetitoKeyVault"
        }
    }

    $result
}

function GetFirstAvailableOrExistingSubnetRange
{
    param (
        [Parameter(Mandatory)] [string] $Environment,
        [Parameter(Mandatory)] [string] $Version
    )

    $AlreadyExistingIpRange = GetAddressPrefix $Environment $Version

    if (![string]::IsNullOrEmpty($AlreadyExistingIpRange))
    {
        $AlreadyExistingIpRange
        return
    }

    $availableIp = GetAvailableIp $Environment
    $availableIpRange = $availableIp + "/28";

    $availableIpRange
}

function GetAvailableIp
{
    param (
        [Parameter(Mandatory)] [string] $Environment
    )

    $EnvironmentIpByte = GetEnvironmentIpByte $Environment

    $possibleIps = @();
    for ($thridByte = 14; $thridByte -le 15; $thridByte++) {
        for ($fourthByte = 0; $fourthByte -le 240; $fourthByte = $fourthByte + 16) {
            $possibleIps += "10.$EnvironmentIpByte.$thridByte.$fourthByte"
        }
    }

    $ResourceGroupName = $Environment + "_meinapetito"
    $VirtualNetworkName = $Environment + "_meinapetito"

    $clusterSubnets = $( az network vnet subnet list -g $ResourceGroupName --vnet-name $VirtualNetworkName --out tsv --query "[?starts_with(addressPrefix, '10.$EnvironmentIpByte.14') || starts_with(addressPrefix, '10.$EnvironmentIpByte.15')].[addressPrefix]" )
    $occupiedIps = $clusterSubnets | ForEach-Object { ($_ -split "/")[0] }
    $freeIps = $possibleIps | Where-Object { $_ -notin $occupiedIps }
    $nextFreeIp = $freeIps[0]

    $nextFreeIp
}

function GetLoadBalancerIp
{
    param (
        [Parameter(Mandatory)] [string] $Environment,
        [Parameter(Mandatory)] [string] $Version
    )

    $ipAddressPrefix = GetAddressPrefix $Environment $Version
    $ipAddressBytes = [System.Net.IPAddress]::Parse(($ipAddressPrefix -split "/")[0]).GetAddressBytes()
    $loadBalancerFourthByte = $ipAddressBytes[3] + 13
    $loadBalancerIp = $ipAddressBytes[0].ToString() + "." + $ipAddressBytes[1].ToString() + "." + $ipAddressBytes[2].ToString() + "." + $loadBalancerFourthByte.ToString()

    $loadBalancerIp
}

function GetAddressPrefix
{
    param (
        [Parameter(Mandatory)] [string] $Environment,
        [Parameter(Mandatory)] [string] $Version
    )

    $ResourceGroupName = $Environment + "_meinapetito"
    $VirtualNetworkName = $Environment + "_meinapetito"
    $SubnetName = $VirtualNetworkName + "_$Version"

    $ExistingSubnets = az network vnet show -g $ResourceGroupName --name $VirtualNetworkName --out tsv --query subnets[*].name

    if ($ExistingSubnets -contains $SubnetName)
    {
        $( az network vnet subnet show -g $ResourceGroupName --vnet-name $VirtualNetworkName --name $SubnetName --out tsv --query addressPrefix )

        return
    }

    ""
}