import environmentVariables from "../../_environmentVariables";

const getEnvironmentVariableHandler = {
  get: function (target, prop) {
    const value = target[prop];

    if (!value) {
      console.log(`Configuration: Value for "${prop}" is not defined`);
      return;
    }
    if (`${value}`.startsWith("$PORTAL_SDK_COMMON")) {
      const envValue = process.env[prop];
      if (envValue) {
        return envValue;
      } else {
        console.log(
          `Configuration: Environment variable "${prop}" is not defined`
        );
      }
    }

    return value;
  },
};

const env = new Proxy(
  { ...environmentVariables },
  getEnvironmentVariableHandler
);
export default env;
