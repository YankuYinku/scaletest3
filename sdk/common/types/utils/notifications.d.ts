export declare interface NotificationsOptions {
    id?: number;
    title?: string;
    text?: string;
    type?: string;
    group?: string;
    duration?: number;
    speed?: number;
    data?: unknown;
    clean?: boolean;
    clear?: boolean;
    ignoreDuplicates?: boolean;
}
export declare const DEFAULT_MESSAGES_NAMESPACE: string;
export declare const useMessage: (namespace?: string) => {
    dispatchMessage: (message: NotificationsOptions) => void;
    dispatchSuccessMessage: (message: Omit<NotificationsOptions, "type">) => void;
    dispatchErrorMessage: (message: Omit<NotificationsOptions, "type">) => void;
    dispatchWarnMessage: (message: Omit<NotificationsOptions, "type">) => void;
    dispatchInfoMessage: (message: Omit<NotificationsOptions, "type">) => void;
};
declare const _default: {
    useMessage: (namespace?: string) => {
        dispatchMessage: (message: NotificationsOptions) => void;
        dispatchSuccessMessage: (message: Omit<NotificationsOptions, "type">) => void;
        dispatchErrorMessage: (message: Omit<NotificationsOptions, "type">) => void;
        dispatchWarnMessage: (message: Omit<NotificationsOptions, "type">) => void;
        dispatchInfoMessage: (message: Omit<NotificationsOptions, "type">) => void;
    };
};
export default _default;
//# sourceMappingURL=notifications.d.ts.map