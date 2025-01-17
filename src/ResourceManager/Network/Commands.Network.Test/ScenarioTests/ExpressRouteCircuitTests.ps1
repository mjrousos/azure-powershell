﻿# ----------------------------------------------------------------------------------
#
# Copyright Microsoft Corporation
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
# http://www.apache.org/licenses/LICENSE-2.0
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
# ----------------------------------------------------------------------------------

<#
.SYNOPSIS
Tests ExpressRouteCircuitCRUD.
#>
function Test-ExpressRouteCircuitCRUD
{
    # Setup
    $rgname = Get-ResourceGroupName
    $circuitName = Get-ResourceName
    $rglocation = Get-ProviderLocation ResourceManagement
    $resourceTypeParent = "Microsoft.Network/expressRouteCircuits"
    $location = Get-ProviderLocation $resourceTypeParent
    $location = "brazilSouth"
    try 
    {
        # Create the resource group
        $resourceGroup = New-AzureRmResourceGroup -Name $rgname -Location $rglocation
        
        # Create the ExpressRouteCircuit
		$circuit = New-AzureRmExpressRouteCircuit -Name $circuitName -Location $location -ResourceGroupName $rgname -SkuTier Standard -SkuFamily MeteredData -BillingType MeteredData -ServiceProviderName "equinix" -PeeringLocation "Silicon Valley" -BandwidthInMbps 1000;
        
        # get Circuit
        $getCircuit = Get-AzureRmExpressRouteCircuit -Name $circuitName -ResourceGroupName $rgname

        #verification
        Assert-AreEqual $rgName $getCircuit.ResourceGroupName
        Assert-AreEqual $circuitName $getCircuit.Name
        Assert-NotNull $getCircuit.Location
        Assert-NotNull $getCircuit.Etag
        Assert-AreEqual 0 @($getCircuit.Peerings).Count
        Assert-AreEqual "Standard_MeteredData" $getCircuit.Sku.Name
        Assert-AreEqual "Standard" $getCircuit.Sku.Tier
        Assert-AreEqual "MeteredData" $getCircuit.Sku.Family
        Assert-AreEqual "equinix" $getCircuit.ServiceProviderProperties.ServiceProviderName
        Assert-AreEqual "Silicon Valley" $getCircuit.ServiceProviderProperties.PeeringLocation
        Assert-AreEqual "1000" $getCircuit.ServiceProviderProperties.BandwidthInMbps

        # list
        $list = Get-AzureRmExpressRouteCircuit -ResourceGroupName $rgname
        Assert-AreEqual 1 @($list).Count
        Assert-AreEqual $list[0].ResourceGroupName $getCircuit.ResourceGroupName
        Assert-AreEqual $list[0].Name $getCircuit.Name
        Assert-AreEqual $list[0].Location $getCircuit.Location
        Assert-AreEqual $list[0].Etag $getCircuit.Etag
        Assert-AreEqual @($list[0].Peerings).Count @($getCircuit.Peerings).Count

		# set
		$getCircuit.ServiceProviderProperties.BandwidthInMbps = 500

		$getCircuit = Set-AzureRmExpressRouteCircuit -ExpressRouteCircuit $getCircuit -BillingType UnlimitedData
		Assert-AreEqual $rgName $getCircuit.ResourceGroupName
        Assert-AreEqual $circuitName $getCircuit.Name
        Assert-NotNull $getCircuit.Location
        Assert-NotNull $getCircuit.Etag
        Assert-AreEqual 0 @($getCircuit.Peerings).Count
        Assert-AreEqual "standard_meteredData" $getCircuit.Sku.Name
        Assert-AreEqual "Standard" $getCircuit.Sku.Tier
        Assert-AreEqual "MeteredData" $getCircuit.Sku.Family
        Assert-AreEqual "equinix" $getCircuit.ServiceProviderProperties.ServiceProviderName
        Assert-AreEqual "Silicon Valley" $getCircuit.ServiceProviderProperties.PeeringLocation
        Assert-AreEqual "500" $getCircuit.ServiceProviderProperties.BandwidthInMbps

        # Delete Circuit
        $delete = Remove-AzureRmExpressRouteCircuit -ResourceGroupName $rgname -name $circuitName -PassThru -Force
        Assert-AreEqual true $delete
		        
        $list = Get-AzureRmExpressRouteCircuit -ResourceGroupName $rgname
        Assert-AreEqual 0 @($list).Count
    }
    finally
    {
        # Cleanup
        Clean-ResourceGroup $rgname
    }
}

<#
.SYNOPSIS
Tests ExpressRouteCircuitPeeringCRUD.
#>
function Test-ExpressRouteCircuitPeeringCRUD
{
    # Setup
    $rgname = Get-ResourceGroupName
    $circuitName = Get-ResourceName
	$rglocation = Get-ProviderLocation ResourceManagement
    $resourceTypeParent = "Microsoft.Network/expressRouteCircuits"
    $location = Get-ProviderLocation $resourceTypeParent
    $location = "brazilSouth"
    try 
    {
        # Create the resource group
        $resourceGroup = New-AzureRmResourceGroup -Name $rgname -Location $rglocation
        
        # Create the ExpressRouteCircuit with peering
		$peering = New-AzureRmExpressRouteCircuitPeeringConfig -Name AzurePrivatePeering -PeeringType AzurePrivatePeering -PeerASN 100 -PrimaryPeerAddressPrefix "192.168.1.0/30" -SecondaryPeerAddressPrefix "192.168.2.0/30" -VlanId 200
		$circuit = New-AzureRmExpressRouteCircuit -Name $circuitName -Location $location -ResourceGroupName $rgname -SkuTier Standard -SkuFamily MeteredData -BillingType UnlimitedData -ServiceProviderName "equinix" -PeeringLocation "Silicon Valley" -BandwidthInMbps 1000 -Peering $peering
        
        #verification
        Assert-AreEqual $rgName $circuit.ResourceGroupName
        Assert-AreEqual $circuitName $circuit.Name
        Assert-NotNull $circuit.Location
        Assert-NotNull $circuit.Etag
        Assert-AreEqual 1 @($circuit.Peerings).Count
        Assert-AreEqual "Standard_MeteredData" $circuit.Sku.Name
        Assert-AreEqual "Standard" $circuit.Sku.Tier
        Assert-AreEqual "MeteredData" $circuit.Sku.Family
        Assert-AreEqual "equinix" $circuit.ServiceProviderProperties.ServiceProviderName
        Assert-AreEqual "Silicon Valley" $circuit.ServiceProviderProperties.PeeringLocation
        Assert-AreEqual "1000" $circuit.ServiceProviderProperties.BandwidthInMbps
		
		# Verify the peering
		Assert-AreEqual "AzurePrivatePeering" $circuit.Peerings[0].Name
		Assert-AreEqual "AzurePrivatePeering" $circuit.Peerings[0].PeeringType
		Assert-AreEqual "100" $circuit.Peerings[0].PeerASN
		Assert-AreEqual "192.168.1.0/30" $circuit.Peerings[0].PrimaryPeerAddressPrefix
		Assert-AreEqual "192.168.2.0/30" $circuit.Peerings[0].SecondaryPeerAddressPrefix
		Assert-AreEqual "200" $circuit.Peerings[0].VlanId

		# get peering
		$p = $circuit | Get-AzureRmExpressRouteCircuitPeeringConfig -Name AzurePrivatePeering
		Assert-AreEqual "AzurePrivatePeering" $p.Name
		Assert-AreEqual "AzurePrivatePeering" $p.PeeringType
		Assert-AreEqual "100" $p.PeerASN
		Assert-AreEqual "192.168.1.0/30" $p.PrimaryPeerAddressPrefix
		Assert-AreEqual "192.168.2.0/30" $p.SecondaryPeerAddressPrefix
		Assert-AreEqual "200" $p.VlanId
		Assert-Null $p.MicrosoftPeeringConfig

		# List peering
		$listPeering = $circuit | Get-AzureRmExpressRouteCircuitPeeringConfig
		Assert-AreEqual 1 @($listPeering).Count

		# add a new Peering
		$circuit = Get-AzureRmExpressRouteCircuit -Name $circuitName -ResourceGroupName $rgname | Add-AzureRmExpressRouteCircuitPeeringConfig -Name MicrosoftPeering -PeeringType MicrosoftPeering -PeerASN 99 -PrimaryPeerAddressPrefix "192.168.1.0/30" -SecondaryPeerAddressPrefix "192.168.2.0/30" -VlanId 200 -MicrosoftConfigAdvertisedPublicPrefixes @("11.2.3.4/30", "12.2.3.4/30") -MicrosoftConfigCustomerAsn 1000 -MicrosoftConfigRoutingRegistryName AFRINIC | Set-AzureRmExpressRouteCircuit -BillingType UnlimitedData

		$p = $circuit | Get-AzureRmExpressRouteCircuitPeeringConfig -Name MicrosoftPeering
		Assert-AreEqual "MicrosoftPeering" $p.Name
		Assert-AreEqual "MicrosoftPeering" $p.PeeringType
		Assert-AreEqual "99" $p.PeerASN
		Assert-AreEqual "192.168.1.0/30" $p.PrimaryPeerAddressPrefix
		Assert-AreEqual "192.168.2.0/30" $p.SecondaryPeerAddressPrefix
		Assert-AreEqual "200" $p.VlanId
		Assert-NotNull $p.MicrosoftPeeringConfig
		Assert-AreEqual "1000" $p.MicrosoftPeeringConfig.CustomerASN
		Assert-AreEqual "AFRINIC" $p.MicrosoftPeeringConfig.RoutingRegistryName
		Assert-AreEqual 2 @($p.MicrosoftPeeringConfig.AdvertisedPublicPrefixes).Count
		Assert-NotNull $p.MicrosoftPeeringConfig.AdvertisedPublicPrefixesState

		$listPeering = $circuit | Get-AzureRmExpressRouteCircuitPeeringConfig
		Assert-AreEqual 2 @($listPeering).Count

		# Set a new peering
	$circuit = Get-AzureRmExpressRouteCircuit -Name $circuitName -ResourceGroupName $rgname | Set-AzureRmExpressRouteCircuitPeeringConfig -Name MicrosoftPeering -PeeringType MicrosoftPeering -PeerASN 100 -PrimaryPeerAddressPrefix "192.168.1.0/30" -SecondaryPeerAddressPrefix "192.168.2.0/30" -VlanId 200 -MicrosoftConfigAdvertisedPublicPrefixes @("11.2.3.4/30", "12.2.3.4/30") -MicrosoftConfigCustomerAsn 1000 -MicrosoftConfigRoutingRegistryName AFRINIC | Set-AzureRmExpressRouteCircuit -BillingType UnlimitedData
		$p = $circuit | Get-AzureRmExpressRouteCircuitPeeringConfig -Name MicrosoftPeering
		Assert-AreEqual "MicrosoftPeering" $p.Name
		Assert-AreEqual "MicrosoftPeering" $p.PeeringType
		Assert-AreEqual "100" $p.PeerASN
		Assert-AreEqual "192.168.1.0/30" $p.PrimaryPeerAddressPrefix
		Assert-AreEqual "192.168.2.0/30" $p.SecondaryPeerAddressPrefix
		Assert-AreEqual "200" $p.VlanId
		Assert-NotNull $p.MicrosoftPeeringConfig
		Assert-AreEqual "1000" $p.MicrosoftPeeringConfig.CustomerASN
		Assert-AreEqual "AFRINIC" $p.MicrosoftPeeringConfig.RoutingRegistryName
		Assert-AreEqual 2 @($p.MicrosoftPeeringConfig.AdvertisedPublicPrefixes).Count
		Assert-NotNull $p.MicrosoftPeeringConfig.AdvertisedPublicPrefixesState

        # Delete Circuit
        $delete = Remove-AzureRmExpressRouteCircuit -ResourceGroupName $rgname -name $circuitName -PassThru -Force
        Assert-AreEqual true $delete
		        
        $list = Get-AzureRmExpressRouteCircuit -ResourceGroupName $rgname
        Assert-AreEqual 0 @($list).Count
    }
    finally
    {
        # Cleanup
        Clean-ResourceGroup $rgname
    }
}