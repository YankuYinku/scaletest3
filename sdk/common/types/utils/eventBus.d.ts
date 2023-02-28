import { Observable } from "windowed-observable";
import { ComponentInternalInstance, Ref } from "vue";
import { Action } from "../models/action";
export declare type EventBusComposable<T> = {
    subscribeEventBusAction: (callback: (event: Action<T> | undefined) => void) => () => boolean;
    loading: Ref<boolean>;
    getEventBusPayload: () => Promise<T>;
};
export declare const getEventBusInstance: <T>(namespace: string) => Observable<Action<T>>;
export declare const useEventBus: <T>(namespace: string, context: ComponentInternalInstance | null) => EventBusComposable<T>;
declare const _default: {
    getEventBusInstance: <T>(namespace: string) => Observable<Action<T>>;
    useEventBus: <T_1>(namespace: string, context: ComponentInternalInstance) => EventBusComposable<T_1>;
};
export default _default;
//# sourceMappingURL=eventBus.d.ts.map