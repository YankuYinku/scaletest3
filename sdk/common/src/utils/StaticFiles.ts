import { getApplicationStaticAssetPath as sdkGetApplicationStaticAssetPath } from "@apetito/sdk";

export function getApplicationStaticAssetPath(
  assetPath: string,
  applicationName: string,
  env: string = "production",
  productionPrefix = "apps"
): string {
  return sdkGetApplicationStaticAssetPath(
    assetPath,
    applicationName,
    env,
    productionPrefix
  );
}
