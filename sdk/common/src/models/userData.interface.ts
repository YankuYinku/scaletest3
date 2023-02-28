import { IPermission } from "./permission.interface";
import { ISortiment } from "./sortiment.interface";
import { Supplier } from "./supplier.interface";

export interface IAllUserData {
  contactPortals: string[];
  languageCodes: string[];
  orderSystems: string[];
  effectiveOrderSystems: string[];
  permissions: IPermission[];
  sortiments: ISortiment[];
  administratedCustomerNumbers: number[];
  suppliers: Supplier[];
}

export interface ICurrentUserData {
  customers: IUserData[];
  all: IAllUserData;
  userEmail: string;
}

export interface IUserData {
  customerNumber: number;
  orderSystem: string;
  orderSystems: string[];
  effectiveOrderSystems: string[];
  permissions: IPermission[];
  languageCode: string;
  contactPortal: string;
  sortiments: ISortiment[];
  role: string;
  suppliers: Supplier[];
}
