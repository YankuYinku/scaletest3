import { Observable } from "windowed-observable";
import { ComponentInternalInstance, onBeforeUnmount, Ref, ref } from "vue";
import { Action } from "../models/action";
// create inter-app communication channel

export type EventBusComposable<T> = {
  subscribeEventBusAction: (
    callback: (event: Action<T> | undefined) => void
  ) => () => boolean;
  loading: Ref<boolean>;
  getEventBusPayload: () => Promise<T>;
};

export const getEventBusInstance = <T>(
  namespace: string
): Observable<Action<T>> => {
  return new Observable<Action<T> | undefined>(namespace);
};

export const useEventBus = <T>(
  namespace: string,
  context: ComponentInternalInstance | null
): EventBusComposable<T> => {
  const eventBus = getEventBusInstance<T>(namespace);
  const lastEvent = eventBus.getLastEvent();

  const subscribeEventBusAction = (
    callback: (event: Action<T> | undefined) => void
  ): (() => boolean) => {
    const eventBusSubscriptionHandler = (event: Action<T> | undefined) => {
      typeof callback === "function" && callback(event);

      return true;
    };

    eventBus.subscribe(eventBusSubscriptionHandler);

    // We are returning the hook to unsubscribe the event handler from the bus, when it's no longer needed.
    return () => {
      eventBus.unsubscribe(eventBusSubscriptionHandler);

      return true;
    };
  };

  let unsubscribeEventBusAction: () => unknown;
  const loading = ref(true);

  const getEventBusPayload = () => {
    const timeout = 30 * 1000;

    return new Promise<T>((resolve, reject) => {
      const waitingTimeout = setTimeout(() => {
        return reject();
      }, timeout);

      const eventBusHandler = (event: Action<T> | undefined) => {
        clearTimeout(waitingTimeout);
        loading.value = false;

        if (event?.payload) {
          return resolve(event.payload);
        }

        return reject();
      };

      unsubscribeEventBusAction = subscribeEventBusAction(eventBusHandler);

      if (lastEvent) {
        loading.value = false;

        return resolve(lastEvent.payload);
      }
    });
  };

  onBeforeUnmount(() => {
    if (typeof unsubscribeEventBusAction === "function") {
      unsubscribeEventBusAction();
    }
  }, context);

  return {
    loading,
    getEventBusPayload,
    subscribeEventBusAction,
  };
};

export default {
  getEventBusInstance,
  useEventBus,
};
