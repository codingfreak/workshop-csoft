Create resource group:

	az group create --name rg-comma --location westeurope --tags purpose=workshop customer=commasoft
	az lock create --lock-type CanNotDelete --name NoDelete --resource-group rg-comma

Create storage account:

	az storage account create --name stoddcomma --resource-group rg-comma

Get primary key and connection string:

	az storage account keys list -n stoddcomma --query "[0].[value]" -o tsv
	az storage account show-connection-string -g rg-comma -n stoddcomma

List Blobs in container:

	az storage blob list -c uploads --account-key [KEY] --account-name stoddcomma -o table

Delete resources

	az lock delete --name NoDelete --resource-group rg-comma 
	az group delete --name rg-comma -y