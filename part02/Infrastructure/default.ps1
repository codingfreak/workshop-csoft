# Sample .\default.ps1 -SubscriptionId c764670f-e928-42c2-86c1-e984e524018a -Location westeurope -ResourceGroupName rg-comma-test -IotHubName ioh-comma-test
# c764670f-e928-42c2-86c1-e984e524018a
Param 
(

    [parameter(Position=0, HelpMessage="The unique ID of the subscription in which the resources should live.", Mandatory=$true)]
    [String]
    $SubscriptionId,

    [parameter(Position=1, HelpMessage="The location in the Azure Network.", Mandatory=$true)]
    [String]
    $Location = "westeurope",

    [parameter(Position=2, HelpMessage="The name of the resource group.", Mandatory=$true)]
    [String]
    $ResourceGroupName,

    [parameter(Position=3, HelpMessage="The name of the IoT hub.", Mandatory=$true)]
    [String]
    $IoTHubName
    
)

# Select the subscription
Write-Host "Setting context to subscription..." -NoNewline
try {
    Set-AzContext -Subscription $SubscriptionId | Out-Null
    Write-Host "Done" -ForegroundColor DarkGreen
}
catch {
    Write-Error "Error" -ErrorAction Stop
}

# Resource Group
Write-Host "Creating resource group '$ResourceGroupName'..." -NoNewline
try {
    New-AzResourceGroup -Name $ResourceGroupName -Location $Location -Tag @{purpose="test"} | Out-Null
    Write-Host "Done" -ForegroundColor DarkGreen
}
catch {
    Write-Error "Error" -ErrorAction Stop
}

# Resource Group Lock
Write-Host "Creating resource group lock..." -NoNewline
try {
    New-AzResourceLock -LockName "no-delete" -ResourceGroupName $ResourceGroupName -LockLevel CanNotDelete -Force | Out-Null
    Write-Host "Done" -ForegroundColor DarkGreen
}
catch {
    Write-Error "Error" -ErrorAction Stop
}


#IoT Hub
Write-Host "Creating IoT hub '$ResourceGroupName'..." -NoNewline
try {
    New-AzIotHub -Name $IoTHubName -ResourceGroupName $ResourceGroupName -Location $Location -SkuName S1 -Units 1  | Out-Null
    Write-Host "Done" -ForegroundColor DarkGreen
}
catch {
    Write-Error "Error" -ErrorAction Stop
}

Write-Host "All ressources created successfully." -ForegroundColor DarkGreen