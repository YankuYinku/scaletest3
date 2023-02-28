// Every entry in environmentVariables will be replaced by the value of an equaly named env var at runtime

const environmentVariables = {
  PORTAL_SDK_COMMON_ENVIRONMENT: "$PORTAL_SDK_COMMON_ENVIRONMENT",
  PORTAL_SDK_COMMON_MEINAPETITO_PORTAL_BASE_URL:
    "$PORTAL_SDK_COMMON_MEINAPETITO_PORTAL_BASE_URL",
  PORTAL_SDK_COMMON_APP_INSIGHTS_CONNECTIONSTRING:
    "$PORTAL_SDK_COMMON_APP_INSIGHTS_CONNECTIONSTRING",
};

export default environmentVariables;
