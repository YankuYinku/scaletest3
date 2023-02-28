import { ISortiment } from "./sortiment.interface";
import { Supplier } from "./supplier.interface";
import { IUserData } from "./userData.interface";

export type CustomerChangedActionPayload = {
  customerNumbers: IUserData[];
  sortiments: ISortiment[];
  orderSystems: string[];
  effectiveOrderSystems: string[];
  permissions: string[];
  languageCodes: string[];
  administratedCustomerNumbers: number[];
  userEmail: string;
  suppliers: Supplier[];
};
