import { getEventBusInstance } from "./eventBus";

/*
 * This is a direct copy of an interface from @kyvg/vue3-notification@2.3.4.
 * It might change in the future.
 * */
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

export const DEFAULT_MESSAGES_NAMESPACE: string = "_notifications__default";

export const useMessage = (namespace = DEFAULT_MESSAGES_NAMESPACE) => {
  const eventBus = getEventBusInstance<NotificationsOptions>(namespace);

  const dispatchMessage = (message: NotificationsOptions) => {
    // TODO: Check if notifications work in apps after modifying the payload
    eventBus.dispatch({
      type: "notifications:add",
      payload: message,
    });
  };

  const dispatchSuccessMessage = (
    message: Omit<NotificationsOptions, "type">
  ) => {
    dispatchMessage({
      ...message,
      type: "success",
    });
  };

  const dispatchErrorMessage = (
    message: Omit<NotificationsOptions, "type">
  ) => {
    dispatchMessage({
      ...message,
      type: "error",
    });
  };

  const dispatchWarnMessage = (message: Omit<NotificationsOptions, "type">) => {
    dispatchMessage({
      ...message,
      type: "warn",
    });
  };

  const dispatchInfoMessage = (message: Omit<NotificationsOptions, "type">) => {
    dispatchMessage({
      ...message,
      type: "info",
    });
  };

  return {
    dispatchMessage,
    dispatchSuccessMessage,
    dispatchErrorMessage,
    dispatchWarnMessage,
    dispatchInfoMessage,
  };
};

export default { useMessage };
