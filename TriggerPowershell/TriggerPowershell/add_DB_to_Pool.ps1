# Param(
               
 #               [string]$newDBname                
 #               )
                

$subscriptionId = 'fbe9ed82-2c21-449e-b0d6-3093522e2459'
$resourceGroupName = 'Default-SQL-EastUS'
$location = 'EAST US'
$serverName = 'bhi5g2ajst'
$poolName = 'Hydroportal-Pool-1'
$databaseName = 'mstest7'

#Import-AzureRmContext -Path 'c:\users\mseul\documents\visual studio 2017\Projects\TriggerPowershell\TriggerPowershell\bin\Debug\azureprofile.json'
Login-AzureRmAccount
Select-AzureSubscription -SubscriptionId 'fbe9ed82-2c21-449e-b0d6-3093522e2459'



New-AzureRmSqlDatabase -ResourceGroupName $resourceGroupName -ServerName $serverName -DatabaseName $databaseName -ElasticPoolName $poolName 