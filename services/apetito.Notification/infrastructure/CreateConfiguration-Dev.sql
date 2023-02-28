WITH ConfigurationsTable (Id, Event, Created)
AS
(
              SELECT '6B8A9EE5-8593-48C8-9952-85BACDF029DB', 'SendMeinApetitoInvitationMail', GETDATE()
              union all
              select 'B8D3D907-1AD5-400E-97CF-F153D6CD4E72', 'SendMeinApetitoContactFormMail',GETDATE()
)
INSERT INTO Configurations (Id, Event, Created)
    SELECT Id, Event, Created
    FROM ConfigurationsTable
    WHERE Id NOT IN (SELECT Id FROM Configurations);

WITH ConfigurationChannlesTable (Id, ConfigurationId, Type, Webhook, WebhookAuthenticationType, WebhookAuthenticationCredentials, Created)
AS
(
              SELECT 'E601478F-7E96-423D-9E76-F97AA98E1CE3', '6B8A9EE5-8593-48C8-9952-85BACDF029DB', 'SENDGRID', 'https://meinapetito-feature41941.westeurope.cloudapp.azure.com/api/root/infrastructure/email/{ReferenceId}', 'NONE', '{}', GETDATE()
              union all
              SELECT '452739AB-AF1B-4786-9291-332BCB9A7106','B8D3D907-1AD5-400E-97CF-F153D6CD4E72', 'SENDGRID', 'https://meinapetito-43457.dev.apebs.de/api/root/infrastructure/email/{ReferenceId}','NONE','{}', GETDATE()
)
INSERT INTO ConfigurationChannels (Id, ConfigurationId, Type, Webhook, WebhookAuthenticationType, WebhookAuthenticationCredentials, Created)
    SELECT Id, ConfigurationId, Type, Webhook, WebhookAuthenticationType, WebhookAuthenticationCredentials, Created
    FROM ConfigurationChannlesTable
    WHERE Id NOT IN (SELECT Id FROM ConfigurationChannels);