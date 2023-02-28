import { ISortiment } from "./sortiment.interface";
import { Supplier } from "./supplier.interface";
import { IUserData } from "./userData.interface";
export declare type CustomerChangedActionPayload = {
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
//# sourceMappingURL=customer-changed-action.d.ts.map