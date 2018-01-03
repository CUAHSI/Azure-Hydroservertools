$subscriptionId = 'fbe9ed82-2c21-449e-b0d6-3093522e2459'
$resourceGroupName = 'Default-SQL-EastUS'
$location = 'EAST US'
$serverName = 'bhi5g2ajst'
$copyServerName = 'bhi5g2ajst'
$poolName = 'Hydroportal-Pool-1'
$databaseName = 'ODM_1_1_1_template'
$copyDatabaseName = 'dbtestcopy1'

Login-AzureRmAccount -Path 'C:\Repos\Azure-Hydroservertools\HydroServerTools\Powershell\azureprofile.json'
Set-AzureRmContext -SubscriptionId $subscriptionId



#New-AzureRmSqlDatabase -ResourceGroupName $resourceGroupName -ServerName $serverName -DatabaseName $databaseName -ElasticPoolName $poolName -MaxSizeBytes 100GB
New-AzureRmSqlDatabaseCopy -ResourceGroupName $resourceGroupName -ServerName $serverName -DatabaseName $databaseName -ElasticPoolName $poolName -CopyDatabaseName $copyDatabaseName

