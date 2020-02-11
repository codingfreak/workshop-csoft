Param 
(

    [parameter(Position=0, HelpMessage="The unique ID of the subscription in which the resources live.", Mandatory=$true)]
    [String]
    $SubscriptionId,

    [parameter(Position=2, HelpMessage="The name of the resource group.", Mandatory=$true)]
    [String]
    $ResourceGroupName
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


# Remove resource lock
Write-Host "Removing resource group lock..." -NoNewline
try {
    Remove-AzResourceLock -LockName "no-delete" -ResourceGroupName $ResourceGroupName -Force | Out-Null
    Write-Host "Done" -ForegroundColor DarkGreen
}
catch {
    Write-Error "Error" -ErrorAction Stop
}

# Remove resource lock
Write-Host "Removing resource group..." -NoNewline
try {
    Remove-AzResourceGroup -ResourceGroupName $ResourceGroupName -Force | Out-Null
    Write-Host "Done" -ForegroundColor DarkGreen
}
catch {
    Write-Error "Error" -ErrorAction Stop
}

Write-Host "All ressources removed successfully." -ForegroundColor DarkGreens