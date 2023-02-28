export declare type FeatureResponse = {
    feature: string;
    value: boolean;
};
/**
 * Reads the feature flag values for the given feature flag names and stores them globally for faster access (without API call).
 * @param featureNames The feature flag names which should be read.
 * @returns unique id, which can be used in Vue to trigger a rerender.
 */
export declare function initFeatureFlags(featureNames: string[]): Promise<FeatureResponse[]>;
declare type FeatureHandler = {
    visibility: (name: string, value: boolean) => void;
};
export declare const loadFeatures: (featureName: string | string[], featureComponent: FeatureHandler) => Promise<boolean>;
export {};
//# sourceMappingURL=index.d.ts.map