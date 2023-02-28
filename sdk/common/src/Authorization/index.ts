import { getCurrentUserInformation, getPermissions } from "@apetito/sdk";
import { authentication } from "../Authentication";
import { appInsights } from "../ApplicationInsights";
import { getEventBusInstance } from "../utils/eventBus";
import { ICurrentUserData } from "../models/userData.interface";
import { IPermission } from "../models/permission.interface";
import {
  IApiResponse,
  ResponseMessageType,
} from "../models/apiResponse.interface";

export type UsePermissions = {
  granted: string[];
  hasPermission: (permissionCode: string) => boolean;
  hasPermissions: (permissionCodes: string[]) => boolean;
  hasAnyPermission: (permissionCodes: string[]) => boolean;
};

type MeinApetitoApplication = EntryPoint & {
  requiresLogin: boolean;
  entryPoints?: EntryPoint[];
};

type EntryPoint = {
  name: string;
  path?: string;
  paths?: string[];
  isDefault?: boolean;
  neededPermissions?: string[];
};

export type { EntryPoint, MeinApetitoApplication };

export const getUserDataEventBusInstance = () => {
  return getEventBusInstance<ICurrentUserData>("@apetito/sspa-user-data");
};

export const usePermissions = (): UsePermissions => {
  const userDataEventBus = getUserDataEventBusInstance();
  const { payload } = userDataEventBus.getLastEvent() || {};

  const granted =
    payload?.all.permissions.map(
      (permission: IPermission) => permission.name
    ) ?? [];

  const hasPermission = (permissionCode: string) =>
    granted.includes(permissionCode);
  const hasPermissions = (permissionCodes: string[]) =>
    permissionCodes.every(hasPermission);
  const hasAnyPermission = (permissionCodes: string[]) =>
    permissionCodes.some(hasPermission);

  return {
    granted,
    hasPermission,
    hasPermissions,
    hasAnyPermission,
  };
};

export async function getUserPermissions(customerNumber?: number) {
  const getUserDataEventBusInstance = getEventBusInstance<ICurrentUserData>(
    "@apetito/sspa-user-data"
  );
  const { payload } = getUserDataEventBusInstance.getLastEvent() || {};
  try {
    const bearerToken = await authentication
      .getToken()
      .catch((error: Error) => {
        appInsights.trackEvent({
          name: "Login_Portal_SDK_Authorization_GetToken",
          properties: {
            customerNumbers: payload?.customers,
            error: error,
          },
        });
      });
    if (bearerToken) {
      return await getPermissions(bearerToken, customerNumber);
    }

    return [];
  } catch (error) {
    appInsights.trackEvent({
      name: "Login_Portal_SDK_Authorization_GetUserPermissions",
      properties: {
        customerNumbers: payload?.customers,
        error: error,
      },
    });
    return [];
  }
}

export async function getCurrentUserInfo(
  excludes: string[] = []
): Promise<IApiResponse<ICurrentUserData>> {
  const getUserDataEventBusInstance = getEventBusInstance<ICurrentUserData>(
    "@apetito/sspa-user-data"
  );
  const { payload } = getUserDataEventBusInstance.getLastEvent() || {};
  try {
    const bearerToken = await authentication
      .getToken()
      .catch((error: Error) => {
        appInsights.trackEvent({
          name: "Login_Portal_SDK_Authorization_GetToken",
          properties: {
            customerNumbers: payload?.customers,
            error: error,
          },
        });
      });
    if (bearerToken) {
      const currentUser = await getCurrentUserInformation(
        bearerToken,
        undefined,
        {
          excludes,
        }
      );

      return {
        data: currentUser,
        httpStatusCode: 200,
        messages: [],
        validationErrors: [],
      };
    }
    return {
      data: undefined,
      httpStatusCode: 401,
      messages: [],
      validationErrors: [],
    };
  } catch (error) {
    appInsights.trackEvent({
      name: "Login_Portal_SDK_Authorization_GetCurrentUserInfo",
      properties: {
        customerNumbers: payload?.customers,
        error: error,
      },
    });
    return {
      data: undefined,
      httpStatusCode: 500,
      messages: [
        {
          type: ResponseMessageType.error,
          code: "FailedToLoadCurrentUser",
          messsage: error,
        },
      ],
      validationErrors: [],
    };
  }
}

export type CurrentUserDataLoaderResult = {
  result: ICurrentUserData;
  reload: boolean;
};

export function useCurrentUserDataLoader(excludes?: string[]) {
  let result: ICurrentUserData = undefined;
  let loading: boolean = false;

  const getUserDataEventBusInstance = getEventBusInstance<ICurrentUserData>(
    "@apetito/sspa-user-data"
  );
  const { payload } = getUserDataEventBusInstance.getLastEvent() || {};

  return (reload: boolean = false): Promise<CurrentUserDataLoaderResult> => {
    if (reload) {
      loading = false;
      result = undefined;
    }

    return new Promise((resolve) => {
      if (result === undefined && !loading) {
        loading = true;

        getCurrentUserInfo(excludes)
          .then((res) => {
            resolve({
              result: res.data,
              reload: true,
            });
          })
          .catch((err: Error) => {
            appInsights.trackEvent({
              name: "Login_Portal_SDK_Authorization_UseCurrentUserDataLoader",
              properties: {
                customerNumbers: payload?.customers,
                error: err,
              },
            });
            resolve({
              result: undefined,
              reload: false,
            });
          })
          .finally(() => {
            loading = false;
          });
      } else {
        resolve({
          result,
          reload: false,
        });
      }
    });
  };
}

export function isApplicationGranted(
  meinApetitoApplication: MeinApetitoApplication,
  permissions: string[]
) {
  return (
    (!meinApetitoApplication.neededPermissions &&
      !meinApetitoApplication.entryPoints) ||
    // either permissions nor entryPoints contain any entries
    (meinApetitoApplication.neededPermissions?.length === 0 &&
      !meinApetitoApplication.entryPoints) ||
    // either permissions nor entryPoints contain any entries
    (!meinApetitoApplication.neededPermissions &&
      meinApetitoApplication.entryPoints?.length === 0) ||
    // either permissions nor entryPoints contain any entries
    (meinApetitoApplication.neededPermissions?.length === 0 &&
      meinApetitoApplication.entryPoints?.length === 0) ||
    (meinApetitoApplication.neededPermissions?.length === 0 &&
      meinApetitoApplication.entryPoints?.every(
        (ep) =>
          ep.neededPermissions === undefined ||
          ep.neededPermissions?.length === 0
      )) ||
    (!meinApetitoApplication.neededPermissions &&
      meinApetitoApplication.entryPoints?.every(
        (ep) =>
          ep.neededPermissions === undefined ||
          ep.neededPermissions?.length === 0
      )) ||
    // app is visible if the user has any of the needed permissions
    meinApetitoApplication.neededPermissions?.some((p) =>
      permissions.includes(p)
    ) ||
    // app is visible if the user has of the permissions needed by any entryPoint
    meinApetitoApplication.entryPoints?.some((ep) =>
      ep.neededPermissions?.some((p) => permissions.includes(p))
    )
  );
}

export function isEntryPointGranted(
  entryPoint: EntryPoint,
  permissions: string[]
) {
  return (
    !entryPoint.neededPermissions ||
    entryPoint.neededPermissions?.length === 0 ||
    entryPoint.neededPermissions?.some((p) => permissions.includes(p))
  );
}

export function getGrantedMeinApetitoApplications(
  meinApetitoApplications: MeinApetitoApplication[],
  permissions: string[]
) {
  return meinApetitoApplications
    .filter((meinApetitoApplication) =>
      isApplicationGranted(meinApetitoApplication, permissions)
    )
    .map((a) => {
      return {
        name: a.name,
        requiresLogin: a.requiresLogin,
        neededPermissions: a.neededPermissions,
        entryPoints: a.entryPoints?.filter((ep) =>
          ep.neededPermissions?.some((p) => permissions.includes(p))
        ),
      };
    });
}
