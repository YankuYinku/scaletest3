const { merge } = require("webpack-merge");
const singleSpaDefaults = require("webpack-config-single-spa-ts");

module.exports = (webpackConfigEnv, argv) => {
  const defaultConfig = singleSpaDefaults({
    orgPackagesAsExternal: false,
    orgName: "apetito",
    projectName: "portal-sdk-common",
    webpackConfigEnv,
    argv,
  });

  return merge(defaultConfig, {});
};
