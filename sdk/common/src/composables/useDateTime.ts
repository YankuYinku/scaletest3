import dayjs from "dayjs";
import deLocale from "dayjs/locale/de";
import atLocale from "dayjs/locale/de-at";
import nlLocale from "dayjs/locale/nl";
import customParseFormat from "dayjs/plugin/customParseFormat";
import localizedFormat from "dayjs/plugin/localizedFormat";

dayjs.extend(customParseFormat);
dayjs.extend(localizedFormat);

interface IDateConfig {
  [locale: string]: { localeName: string; localeConfig: ILocale };
}

const dateConfig: IDateConfig = {
  "de-DE": {
    localeName: "de",
    localeConfig: deLocale,
  },
  "de-AT": {
    localeName: "de-at",
    localeConfig: atLocale,
  },
  "nl-NL": {
    localeName: "nl",
    localeConfig: nlLocale,
  },
};

export function useDateTimeFormatter(locale: string) {
  const config = dateConfig[locale] ?? dateConfig["de-DE"];
  dayjs.locale(config.localeConfig, undefined, true);

  function toIsoString(date: string, dateFormat: string = "L", outputFormat: string = "YYYY-MM-DDTHH:mm:ss.SSS") {
    if (date) {
      return dayjs(date, dateFormat, config.localeName).format(outputFormat);
    }
    return "";
  }

  function toLocaleDate(date: string, dateFormat: string) {
    if (date) {
      return dayjs(date, dateFormat, config.localeName).format("L");
    }
    return "";
  }

  return { toIsoString, toLocaleDate };
}
