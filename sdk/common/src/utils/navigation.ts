import { getEventBusInstance, useEventBus } from "./eventBus";
import {
  ComponentInternalInstance,
  onBeforeMount,
  onBeforeUnmount,
  reactive,
  ref,
} from "vue";

export const DEFAULT_NAVIGATION_EVENT = "navigation:set-application-route";

export type NavigationRouteData = {
  applicationName: string;
  pageTitle?: string;
};

export const dispatchNavigationData = (
  applicationName: string,
  pageTitle?: string,
  namespace?: string
) => {
  const eventBus = getEventBusInstance<NavigationRouteData>(
    namespace || DEFAULT_NAVIGATION_EVENT
  );

  eventBus.dispatch({
    type: "navigation:set-application-route",
    payload: {
      applicationName,
      pageTitle,
    },
  });
};

export const useNavigationData = (
  context: ComponentInternalInstance | null,
  onUpdate?: (payload: NavigationRouteData | null) => void,
  namespace = DEFAULT_NAVIGATION_EVENT
) => {
  const eventBus = useEventBus<NavigationRouteData>(namespace, context);
  const data = reactive({
    currentApplicationName: "",
    pageTitle: "",
  });
  // MutationObserver for detecting URI changes (path, hash, query params, etc)
  const currentUri = ref(window.location.href);
  const observer = new MutationObserver(() => {
    const uri = window.location.href;

    if (uri !== currentUri.value) {
      onUpdate(null);
      currentUri.value = uri;
    }
  });

  const setNavigationData = (payload: NavigationRouteData) => {
    data.currentApplicationName = payload.applicationName;
    data.pageTitle = payload.pageTitle;

    typeof onUpdate === "function" && onUpdate(payload);
  };

  const destroySubscription = eventBus.subscribeEventBusAction((event) => {
    setNavigationData(event.payload);
  });

  onBeforeMount(() => {
    eventBus.getEventBusPayload().then((payload) => {
      setNavigationData(payload);
    });

    // The best working solution for detecting URI changes (path, hash, query params, etc)
    observer.observe(document, {
      subtree: true,
      childList: true,
    });
  }, context);

  onBeforeUnmount(() => {
    destroySubscription();
    observer.disconnect();
  }, context);

  return data;
};

export default { dispatchNavigationData, useNavigationData };
