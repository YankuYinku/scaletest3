function SecretExists($secretName, $label) {
    $secretName = $secretName.replace(":", "-")
    if ($null -ne $label) {
        $secretName = $secretName + "-$label"
    }

    Write-Information "Check if secret with name $secretName exists" -InformationAction Continue
    $numberOfSecrets = $(az keyvault secret list --vault-name $KEYVAULT_NAME --query="[?name=='$secretName'] | length(@)")
    return $(if ($numberOfSecrets -gt 0) { $true } else { $false })
}

function SettingExists($settingName, $label) {
    if ($null -eq $label) {
        Write-Information "Check if app setting with name $settingName exists" -InformationAction Continue
        $numberOfSettings = $(az appconfig kv list --name $APPCONFIG_NAME --query="[?key=='$settingName' && label==null] | length(@)")
    } else {
        Write-Information "Check if app setting with name $settingName and label $label exists" -InformationAction Continue
        $numberOfSettings = $(az appconfig kv list --name $APPCONFIG_NAME --query="[?key=='$settingName' && label=='$label'] | length(@)")
    }
    return $(if ($numberOfSettings -gt 0) { $true } else { $false })
}

function ContainsOmittedValue($value){
    if($value -like "*omitted*" -or $value -like "*value ommited*" -or $value -like "*value omited*"){
        return $true
    } else {
        return $false
    }
}

function ForceCreateSettingWithExistingKeyVaultEntry($key, $keyVaultKey, $label=$null) {
    $uri = az keyvault secret show `
                --vault-name $KEYVAULT_NAME `
                --name $keyVaultKey `
                --query=id -o tsv

    #Remove the exact version from the secret reference URI, to always use latest version
    $slashestoremove = 2
    $index = ($uri.split("/").count - $slashestoremove)
    $latestVersionUri = $uri.split("/")[0..$index] -join "/"

    if ($null -eq $label) {
        az appconfig kv set-keyvault `
            --name $APPCONFIG_NAME `
            --key $key `
            --secret-identifier $latestVersionUri `
            --yes
    } else {
        az appconfig kv set-keyvault `
            --name $APPCONFIG_NAME `
            --key $key `
            --label $label `
            --secret-identifier $latestVersionUri `
            --yes
    }
}

function CreateSettingWithExistingKeyVaultEntry($key, $keyVaultKey, $label=$null) {

    if (SettingExists $key $label) {
        Write-Output "Setting with key $key and label $label already exists - skipping creation"
        return
    }

    $uri = az keyvault secret show `
                --vault-name $KEYVAULT_NAME `
                --name $keyVaultKey `
                --query=id -o tsv

    if ($null -eq $uri) {
        Write-Output "Secret with key $keyVaultKey and label $label already exists - skipping creation"
        return
    }

    ForceCreateSettingWithExistingKeyVaultEntry $key $keyVaultKey $label
}

function ForceCreateSecretWithoutAppSetting($key, $value, $label=$null) {
    if(ContainsOmittedValue($value)){
        Write-Output "Cannot set the value: $value for key: $key"
        return
    }

    $keyVaultKey = $key.replace(":", "-")
    if ($null -ne $label) {
        $keyVaultKey = $keyVaultKey + "-$label"
    }

    az keyvault secret set `
        --vault-name $KEYVAULT_NAME `
        --name $keyVaultKey `
        --value $value `
        --query=id -o tsv
}

function CreateSecretWithoutAppSetting($key, $value, $label=$null) {
    $keyVaultKey = $key.replace(":", "-")
    if ($null -ne $label) {
        $keyVaultKey = $keyVaultKey + "-$label"
    }

    if (SecretExists $keyVaultKey) {
        Write-Output "Secret with key $keyVaultKey already exists - skipping creation"
        return
    }

    ForceCreateSecretWithoutAppSetting $key $value $label
}


function ForceCreateSecret($key, $value, $label=$null){
    if(ContainsOmittedValue($value)){
        Write-Output "Cannot set the value: $value for key: $key"
        return
    }

    # Create a secret & link in key vault, even if they already exist (new version gets created)
    $keyVaultKey = $key.replace(":", "-")
    if ($null -ne $label) {
        $keyVaultKey = $keyVaultKey + "-$label"
    }

    $uri = az keyvault secret set `
    --vault-name $KEYVAULT_NAME `
    --name $keyVaultKey `
    --value $value `
    --query=id -o tsv

    #Remove the exact version from the secret reference URI, to always use latest version
    $slashestoremove = 2
    $index = ($uri.split("/").count - $slashestoremove)
    $latestVersionUri = $uri.split("/")[0..$index] -join "/"

    if ($null -eq $label) {
        az appconfig kv set-keyvault `
            --name $APPCONFIG_NAME `
            --key $key `
            --secret-identifier $latestVersionUri `
            --yes
    } else {
        az appconfig kv set-keyvault `
            --name $APPCONFIG_NAME `
            --key $key `
            --label $label `
            --secret-identifier $latestVersionUri `
            --yes
    }
}

function CreateSecret($key, $value, $label=$null) {
    # Create a secret & link in key vault if the secret or setting does not already exist
    $keyVaultKey = $key.replace(":", "-")
    if ($null -ne $label) {
        $keyVaultKey = $keyVaultKey + "-$label"
    }

    Write-Output "$key $label $keyVaultKey $value"

    if (SecretExists $keyVaultKey || SettingExists $key $label) {
        Write-Output "Secret or Setting with key $keyVaultKey already exists - skipping creation"
        return
    }

    ForceCreateSecret $key $value $label
}

function ForceCreateSetting($key, $value, $label=$null) {
    if(ContainsOmittedValue($value)){
        Write-Output "Cannot set the value: $value for key: $key"
        return
    }

    #Creates the setting and overwrites existing settings with a new version if they already exist
    if ($null -eq $label) {
        az appconfig kv set `
            --name $APPCONFIG_NAME `
            --key $key `
            --value $value `
            --yes
    }
    else {
        az appconfig kv set `
            --name $APPCONFIG_NAME `
            --key $key `
            --value $value `
            --label $label `
            --yes
    }
}

function CreateSetting($key, $value, $label=$null) {
    #Creates the setting if no setting with the given name already exists
    if (SettingExists $key $label) {
        Write-Output "Setting with key '$key' and label '$label' already exists - skipping creation"
        return
    }

    ForceCreateSetting $key $value $label
}