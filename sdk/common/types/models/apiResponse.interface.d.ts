export interface IApiResponse<T> {
    data: T;
    httpStatusCode: number;
    messages: IResponseMessage[];
    validationErrors: IResponseValidationErrors[];
}
export interface IResponseMessage {
    code: string;
    messsage?: string;
    type: ResponseMessageType;
}
export interface IResponseValidationErrors {
    propertyName: string;
    attemptedValue: object;
    errorCode: string;
    message: string;
}
export declare enum ResponseMessageType {
    info = 0,
    success = 1,
    warning = 2,
    error = 3
}
//# sourceMappingURL=apiResponse.interface.d.ts.map