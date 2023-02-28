import {
  Authentication,
  DevelopmentStage,
  ProductionStage,
  StagingStage,
} from "@apetito/sdk";

import env from "../utils/env";
import { IAuthenticationOptions } from "@apetito/sdk/dist/models/Authentication";

const stage = env.PORTAL_SDK_COMMON_ENVIRONMENT;

let stageObject: IAuthenticationOptions;
if (stage == "Development") stageObject = DevelopmentStage;
else if (stage == "Staging") stageObject = StagingStage;
else stageObject = ProductionStage;

export const authentication = new Authentication(stageObject);
