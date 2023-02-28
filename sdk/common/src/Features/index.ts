import { authentication } from "../Authentication";
import env from "../utils/env";
import { appInsights } from "../ApplicationInsights";
import { getEventBusInstance } from "../utils/eventBus";
import { ICurrentUserData } from "../models/userData.interface";

export type FeatureResponse = { feature: string; value: boolean };

// Returns the loaded feature flag and their values. In case the GET call returns an error, the feature flag is returned as false (not available).
async function getFeatureFlag(featureName: string): Promise<FeatureResponse> {
  const getUserDataEventBusInstance = getEventBusInstance<ICurrentUserData>(
    "@apetito/sspa-user-data"
  );
  const { payload } = getUserDataEventBusInstance.getLastEvent() || {};

  const headers = new Headers();
  const token = await authentication.getToken().catch((error: Error) => {
    appInsights.trackEvent({
      name: "Login_Portal_SDK_Features_GetToken",
      properties: {
        customerNumbers: payload?.customers,
        error: error,
      },
    });
  });

  headers.append("Content-Type", "application/json");
  headers.append("Authorization", `Bearer ${token}`);

  return (
    fetch(
      `${env.PORTAL_SDK_COMMON_MEINAPETITO_PORTAL_BASE_URL}/root/configuration/features/${featureName}`,
      {
        method: "GET",
        headers: headers,
      }
    )
      .then((response) => response.json())
      // if it could not be loaded, the feature is not available
      .catch((error: Error) => {
        appInsights.trackEvent({
          name: "Login_Portal_SDK_Features_GetFeatureFlag",
          properties: {
            customerNumbers: payload?.customers,
            error: error,
          },
        });
        return { feature: featureName, value: false };
      })
  );
}

/**
 * Reads the feature flag values for the given feature flag names and stores them globally for faster access (without API call).
 * @param featureNames The feature flag names which should be read.
 * @returns unique id, which can be used in Vue to trigger a rerender.
 */
export async function initFeatureFlags(featureNames: string[]) {
  if (featureNames?.length === 0) {
    throw new Error("initFeatureFlags called without feature flag names.");
  }

  const featureFlagSymbol = "__apff"; // apetito feature flag

  // init global feature flag map if not yet initialized.
  if (window[featureFlagSymbol] === undefined) {
    window[featureFlagSymbol] = new Map<string, boolean>();
  }

  const featureFlagMap: Map<string, boolean> = window[featureFlagSymbol];

  // create promises for all feature flags: Return those who are available immediately and load the others.
  const all = featureNames.map<Promise<FeatureResponse>>((featureName) => {
    const featureValue = featureFlagMap.get(featureName);
    return featureValue !== undefined
      ? Promise.resolve<FeatureResponse>({
          feature: featureName,
          value: featureValue,
        })
      : getFeatureFlag(featureName).then((value) => {
          // store value for loaded feature flags for future use.
          featureFlagMap.set(value.feature, value.value);
          return value;
        });
  });
  return Promise.all(all);
}

type FeatureHandler = { visibility: (name: string, value: boolean) => void };

export const loadFeatures = (
  featureName: string | string[],
  featureComponent: FeatureHandler
): Promise<boolean> => {
  const names = !Array.isArray(featureName) ? [featureName] : featureName;

  return new Promise((resolve, reject) => {
    initFeatureFlags(names)
      .then((result: FeatureResponse[]) => {
        result.forEach((rule) => {
          featureComponent.visibility(rule.feature, rule.value);
        });

        resolve(true);
      })
      .catch(() => {
        reject(false);
      });
  });
};
