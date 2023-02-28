import { ICurrentUserData } from "../models/userData.interface";
import { IApiResponse } from "../models/apiResponse.interface";
export declare type UsePermissions = {
    granted: string[];
    hasPermission: (permissionCode: string) => boolean;
    hasPermissions: (permissionCodes: string[]) => boolean;
    hasAnyPermission: (permissionCodes: string[]) => boolean;
};
declare type MeinApetitoApplication = EntryPoint & {
    requiresLogin: boolean;
    entryPoints?: EntryPoint[];
};
declare type EntryPoint = {
    name: string;
    path?: string;
    paths?: string[];
    isDefault?: boolean;
    neededPermissions?: string[];
};
export type { EntryPoint, MeinApetitoApplication };
export declare const getUserDataEventBusInstance: () => import("windowed-observable").Observable<import("../apetito-portal-sdk-common").Action<ICurrentUserData>>;
export declare const usePermissions: () => UsePermissions;
export declare function getUserPermissions(customerNumber?: number): Promise<any>;
export declare function getCurrentUserInfo(excludes?: string[]): Promise<IApiResponse<ICurrentUserData>>;
export declare type CurrentUserDataLoaderResult = {
    result: ICurrentUserData;
    reload: boolean;
};
export declare function useCurrentUserDataLoader(excludes?: string[]): (reload?: boolean) => Promise<CurrentUserDataLoaderResult>;
export declare function isApplicationGranted(meinApetitoApplication: MeinApetitoApplication, permissions: string[]): boolean;
export declare function isEntryPointGranted(entryPoint: EntryPoint, permissions: string[]): boolean;
export declare function getGrantedMeinApetitoApplications(meinApetitoApplications: MeinApetitoApplication[], permissions: string[]): {
    name: string;
    requiresLogin: boolean;
    neededPermissions: string[];
    entryPoints: EntryPoint[];
}[];
//# sourceMappingURL=index.d.ts.map