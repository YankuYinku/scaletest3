import { ApplicationInsights } from "@microsoft/applicationinsights-web";
export declare const appInsights: ApplicationInsights;
export declare type IAppInsightsUserData = {
    orderSystem: string;
    customerNumber: number;
};
export declare function setAppInsightsUser(userData: IAppInsightsUserData): void;
declare const _default: {
    appInsights: ApplicationInsights;
    setAppInsightsUser: typeof setAppInsightsUser;
};
export default _default;
//# sourceMappingURL=index.d.ts.map