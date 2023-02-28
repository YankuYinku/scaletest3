import { ApplicationInsights } from "@microsoft/applicationinsights-web";
import env from "../utils/env";

const appInsightsConnectionString =
  env.PORTAL_SDK_COMMON_APP_INSIGHTS_CONNECTIONSTRING;

export const appInsights = new ApplicationInsights({
  config: {
    connectionString: appInsightsConnectionString,
    enableAutoRouteTracking: true,
  },
});

appInsights.loadAppInsights();

export type IAppInsightsUserData = {
  orderSystem: string;
  customerNumber: number;
};

export function setAppInsightsUser(userData: IAppInsightsUserData) {
  appInsights.config.accountId = userData.customerNumber.toString();
  appInsights.addTelemetryInitializer((envelope) => {
    envelope.tags["ai.cloud.role"] = userData.orderSystem;
    envelope.tags["ai.cloud.roleInstance"] = userData.customerNumber.toString();
  });
  appInsights.trackPageView();
}

export default {
  appInsights,
  setAppInsightsUser,
};
