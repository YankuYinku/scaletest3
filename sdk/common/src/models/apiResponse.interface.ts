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

export enum ResponseMessageType {
  info,
  success,
  warning,
  error,
}
