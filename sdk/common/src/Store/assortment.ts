const ASSORTMENT_SELECT = "assortment:select";
const SELECTED_ASSORTMENTS = "selectedAssortments";

const subscribeToAssortmentSelect = (fn) => {
  window.addEventListener(ASSORTMENT_SELECT, (e) => fn(e));
};

const unsubscribeAssortmentSelect = (fn) => {
  window.removeEventListener(ASSORTMENT_SELECT, fn);
};

const selectAssortment = (assortment) => {
  const event = new CustomEvent(ASSORTMENT_SELECT, {
    bubbles: true,
    detail: { items: assortment },
  });

  localStorage.setItem(SELECTED_ASSORTMENTS, JSON.stringify(assortment));
  window.dispatchEvent(event);
};

const selectedAssortments = () => {
  return JSON.parse(localStorage.getItem(SELECTED_ASSORTMENTS)) || [];
};

export const assortment = {
  selectedAssortments,
  selectAssortment,
  subscribeToAssortmentSelect,
  unsubscribeAssortmentSelect,
};
