export const getImprintUrl = () => {
  const imprintUrls = {
    "de-DE": "https://www.apetito.de/impressum",
    "de-AT": "https://www.apetito.co.at/impressum",
    "nl-NL": "https://www.apetito.nl/service/algemene-voorwaarden.html",
  };
  const fallBackUrl = "https://www.apetito.de/impressum";
  const domain = getLanguageCodeFromUrl();

  return imprintUrls[domain[1]] ?? fallBackUrl;
};

export const getLanguageCodeFromUrl = () => {
  const topLevelDomain = window.location.hostname.match(".([^\\.]+?)$");
  const languageCodes = {
    de: "de-DE",
    at: "de-AT",
    nl: "nl-NL",
  };

  if (!topLevelDomain || topLevelDomain.length < 2) {
    return languageCodes.de;
  }

  return languageCodes[topLevelDomain[1]] ?? languageCodes.de;
};