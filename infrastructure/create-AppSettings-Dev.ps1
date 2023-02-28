$prevPwd = $PWD; Set-Location -ErrorAction Stop -LiteralPath $PSScriptRoot

./../../../infrastructure/scripts/create-appSettings.ps1

$APPCONFIG_NAME = "apetitoAppSettings-development"; # "apetitoAppSettings-staging" | "apetitoAppSettings" 
$KEYVAULT_NAME = "apetitoKeyVault-dev"; # "apetitoKeyVault-staging" | "apetitoKeyVault" 

CreateSettingWithExistingKeyVaultEntry "apetito:meinapetito:portal:api:AzureADB2C:AppRegistration:PortalManagement:ClientID" "apetito-AzureADB2C-AppRegistration-PortalManagement-ClientId"
CreateSettingWithExistingKeyVaultEntry "apetito:meinapetito:portal:api:AzureADB2C:AppRegistration:PortalManagement:ClientSecret" "apetito-AzureADB2C-AppRegistration-PortalManagement-ClientSecret"
CreateSettingWithExistingKeyVaultEntry "apetito:meinapetito:portal:api:AzureADB2C:TenantID" "apetito-AzureADB2C-TenantId"
CreateSettingWithExistingKeyVaultEntry "apetito:meinapetito:portal:api:AzureADB2C:Domain" "apetito-AzureADB2C-Domain"
CreateSecret "apetito:meinapetito:portal:api:AzureADB2C:PostRegistrationWebhook:Secret" "<Value omitted>"

CreateSecret "apetito:meinapetito:portal:api:ConnectionStrings:DistributedCache" "<Value omitted>"

CreateSetting "apetito:meinapetito:portal:api:Email:ApetitoNotificationBaseUrl" "http://notification-api"
CreateSetting "apetito:meinapetito:portal:api:Email:ChannelType" "SENDGRID"
CreateSetting "apetito:meinapetito:portal:api:Email:EventName" "SendMeinApetitoInvitationMail"
CreateSetting "apetito:meinapetito:portal:api:Email:InvitationSenderEmailAddress" "mein-apetito@apebs.de"

CreateSetting "apetito:meinapetito:portal:api:ProductCatalog:ApetitoIProdaApi:Address" "https://api.apetito.de/article/iproda/"
CreateSetting "apetito:meinapetito:portal:api:ProductCatalog:ApetitoArticleGatewayApi:Address" "https://api.apetito.de/articlegateway/"
CreateSetting "apetito:meinapetito:portal:api:ProductCatalog:MeinApetitoApi:Address" "https://staging.meinapetito.de/api/"
CreateSetting "apetito:meinapetito:portal:api:ProductCatalog:PhotoBuilder:ProductImagePath" "https://www.apetito-menues.de/mediaport/mylunch_Speiseplan_{0}/"
CreateSetting "apetito:meinapetito:portal:api:ProductCatalog:PhotoBuilder:NutriScoreImagePath" "https://www.apetito-menues.de/media_index/Artikel/nutriscore/svg/border/NutriScore{0}_B.svg"

CreateSetting "apetito:meinapetito:portal:api:Dashbaord:Prismic:Documents:RemnantsSliderId" "YXgD8hcAAC8ABmBp"

CreateSetting "apetito:meinapetito:portal:api:News:Prismic:DocumentTypeName" "news"
CreateSetting "apetito:meinapetito:portal:api:News:Prismic:CategoryDocumentTypeName" "articel_category"

CreateSetting "apetito:meinapetito:portal:api:Prismic:Endpoint" "https://meinapetito-development.prismic.io/api/v1"
CreateSecret "apetito:meinapetito:portal:api:Prismic:AccessToken" "<Value omitted>"
CreateSecret "apetito:meinapetito:portal:api:Prismic:Webhook:Secret" "<Value omitted>" 



$prevPwd | Set-Location