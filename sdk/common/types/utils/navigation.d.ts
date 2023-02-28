import { ComponentInternalInstance } from "vue";
export declare const DEFAULT_NAVIGATION_EVENT = "navigation:set-application-route";
export declare type NavigationRouteData = {
    applicationName: string;
    pageTitle?: string;
};
export declare const dispatchNavigationData: (applicationName: string, pageTitle?: string, namespace?: string) => void;
export declare const useNavigationData: (context: ComponentInternalInstance | null, onUpdate?: (payload: NavigationRouteData | null) => void, namespace?: string) => {
    currentApplicationName: string;
    pageTitle: string;
};
declare const _default: {
    dispatchNavigationData: (applicationName: string, pageTitle?: string, namespace?: string) => void;
    useNavigationData: (context: ComponentInternalInstance, onUpdate?: (payload: NavigationRouteData) => void, namespace?: string) => {
        currentApplicationName: string;
        pageTitle: string;
    };
};
export default _default;
//# sourceMappingURL=navigation.d.ts.map