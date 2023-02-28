import { assortment } from "./assortment";
const createStore = () => {
  return {
    ...assortment,
  };
};

export const store = createStore();
