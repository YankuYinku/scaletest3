import { getLegacyBearerToken } from "@apetito/sdk";
import { authentication } from "../Authentication";
import { appInsights } from "../ApplicationInsights";
import { getEventBusInstance } from "../utils/eventBus";
import { ICurrentUserData } from "../models/userData.interface";

const createHiddenInputElement = (token) => {
  const input = document.getElementById(
    "hiddenBearerToken"
  ) as HTMLInputElement;
  const event = new Event("input");
  input.setAttribute("type", "hidden");
  input.setAttribute("id", "hiddenBearerToken");
  input.setAttribute("name", "hiddenBearerToken");
  input.setAttribute("value", token);
  input.dispatchEvent(event);
};

export async function getBearerToken(): Promise<string> {
  const getUserDataEventBusInstance = getEventBusInstance<ICurrentUserData>(
    "@apetito/sspa-user-data"
  );
  const { payload } = getUserDataEventBusInstance.getLastEvent() || {};

  const bearerToken = await authentication.getToken().catch((error: Error) => {
    appInsights.trackEvent({
      name: "Login_Portal_SDK_BearerTokenHelper_GetToken",
      properties: {
        customerNumbers: payload?.customers,
        error: error,
      },
    });
  });
  if (bearerToken) {
    const result = await getLegacyBearerToken(
      bearerToken,
      "/api/root/authentication/userAccessTokens/legacy"
    );
    if (result?.token) {
      createHiddenInputElement(result.token);
      return result.token;
    }
  }

  return "";
}
