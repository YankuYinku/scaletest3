import { getIbsscToken } from "@apetito/sdk";
import { authentication } from "../Authentication";
import env from "../utils/env";
import config from "../sdk.config";
import { appInsights } from "../ApplicationInsights";
import { getEventBusInstance } from "../utils/eventBus";
import { ICurrentUserData } from "../models/userData.interface";

export async function getIbsscUrl() {
  const getUserDataEventBusInstance = getEventBusInstance<ICurrentUserData>(
    "@apetito/sspa-user-data"
  );
  const { payload } = getUserDataEventBusInstance.getLastEvent() || {};

  const bearerToken = await authentication.getToken().catch((error: Error) => {
    appInsights.trackEvent({
      name: "Login_Portal_SDK_IbsscHelper_GetToken",
      properties: {
        customerNumbers: payload?.customers,
        error: error,
      },
    });
  });
  if (bearerToken) {
    const result = await getIbsscToken(bearerToken);
    if (result?.token) {
      try {
        const email = authentication.getEmail();
        const ibsscUrl = `${getBaseUrl()}/Login?email=${email}&token=${
          result.token
        }&redirectUri=${encodeURI(window.location.origin)}`;
        return ibsscUrl;
      } catch (error) {
        appInsights.trackEvent({
          name: "Login_Portal_SDK_IbsscHelper_GetEmail",
          properties: {
            customerNumbers: payload?.customers,
            error: error,
          },
        });
      }
    }
  }
  return "";
}

const stage = env.PORTAL_SDK_COMMON_ENVIRONMENT;

function getBaseUrl() {
  if (stage == "Development") return config.ibssc.development;
  else if (stage == "Staging") return config.ibssc.staging;
  else return config.ibssc.production;
}
