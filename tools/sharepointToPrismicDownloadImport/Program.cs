using System.Text.Json;
using System.Runtime;
using Microsoft.WindowsAPICodePack.Shell;
using System.Drawing;
using System.Drawing.Imaging;
using JsonDownload;
using static JsonDownload.ThumbnailExtractor;
using System.Web;
using System.Text.RegularExpressions;
using System.Net;
using System.Text.Json.Serialization;

//this is a hacky script to get downloads from a sharepoint json, create thumbnails and convert them to prismic json
//manual intervention and configuring is required

#region intro
Console.WriteLine("  _________.__                                       .__        __                                            ");
Console.WriteLine(" /   _____/|  |__ _____ _______   ____ ______   ____ |__| _____/  |_                                          ");
Console.WriteLine(" \\_____  \\ |  |  \\\\__  \\\\_  __ \\_/ __ \\\\____ \\ /  _ \\|  |/    \\   __\\                            ");
Console.WriteLine(" /        \\|   Y  \\/ __ \\|  | \\/\\  ___/|  |_> >  <_> )  |   |  \\  |                                     ");
Console.WriteLine("/_______  /|___|  (____  /__|    \\___  >   __/ \\____/|__|___|  /__|                                         ");
Console.WriteLine("        \\/      \\/     \\/            \\/|__|                  \\/                                          ");
Console.WriteLine("                                      ________                      .__                    .___               ");
Console.WriteLine("                                      \\______ \\   ______  _  ______ |  |   _________     __| _/             ");
Console.WriteLine("                                       |    |  \\ /  _ \\ \\/ \\/ /    \\|  |  /  _ \\__  \\   / __ |         ");
Console.WriteLine("                                       |    `   (  <_> )     /   |  \\  |_(  <_> ) __ \\_/ /_/ |              ");
Console.WriteLine("                                      /_______  /\\____/ \\/\\_/|___|  /____/\\____(____  /\\____ |           ");
Console.WriteLine("                                              \\/                  \\/                \\/      \\/            ");
Console.WriteLine("                                                                                ___________           .__     ");
Console.WriteLine("                                                                                \\__    ___/___   ____ |  |   ");
Console.WriteLine("                                                                                  |    | /  _ \\ /  _ \\|  |  ");
Console.WriteLine("                                                                                  |    |(  <_> |  <_> )  |__  ");
Console.WriteLine("                                                                                  |____| \\____/ \\____/|____/");
#endregion

#region setting up the stage
//default settings
var pathToFullJson = "C:\\json\\Downloads_DE_PROD.json";
var meinapetitoBaseUrl = "https://www.meinapetito.de";
var languageCode = "de-de";
var fedAuthToken = ""; //copy from sharepoint request (cookie in request header), depending on environment
var downloadFilesFromSharepoint = false;
var createThumbnailForDownloads = false;
var createJsonFileForDownloads = true;
var failedDownloads = new List<Tuple<string, string>>();

Console.WriteLine("Make sure you have created this folder --> C:\\json\\output and set the fedAuth Token in the settings");
Console.WriteLine("Also dont run this as admin, because windows cant get thumbnails that way?! (dont ask)");
Console.WriteLine($"Enter path to downloads.json (Default: {pathToFullJson}): ");
var path = Console.ReadLine();
if (!String.IsNullOrEmpty(path))
    pathToFullJson = path;

Console.WriteLine($"Enter meinapetito base url (Default: {meinapetitoBaseUrl}): ");
var url = Console.ReadLine();
if (!String.IsNullOrEmpty(url))
    meinapetitoBaseUrl = url;

Console.WriteLine($"Enter language code (Default: {languageCode}): ");
var language = Console.ReadLine();
if (!String.IsNullOrEmpty(language))
    languageCode = language;

Console.WriteLine($"Enter FedAuth Token (Default: {fedAuthToken}): ");
var fedAuthy = Console.ReadLine();
if (!String.IsNullOrEmpty(fedAuthy))
    fedAuthToken = fedAuthy;

Console.WriteLine($"Download files fresh from sharepoint? (Default: {downloadFilesFromSharepoint}): ");
var downloadNow = Console.ReadLine();
if (!String.IsNullOrEmpty(downloadNow) && Boolean.TryParse(downloadNow, out bool downloadResult))
    downloadFilesFromSharepoint = downloadResult;

Console.WriteLine($"Create Thumbnails? (Default: {createThumbnailForDownloads}): ");
var createThumbs = Console.ReadLine();
if (!String.IsNullOrEmpty(createThumbs) && Boolean.TryParse(createThumbs, out bool thumbResult))
    createThumbnailForDownloads = thumbResult;

Console.WriteLine($"Create JSON files? (Default: {createJsonFileForDownloads}): ");
var createJson = Console.ReadLine();
if (!String.IsNullOrEmpty(downloadNow) && Boolean.TryParse(downloadNow, out bool jsonResult))
    createJsonFileForDownloads = jsonResult;
#endregion

var jsonString = File.ReadAllText(pathToFullJson);
var parsed = JsonSerializer.Deserialize<MeinapetitoDownload>(jsonString);

if (parsed?.DownloadList != null)
{
    var run = 0;
    foreach (var item in parsed.DownloadList)
    {
        try
        {
            Console.WriteLine($"Starting file: {item.Title}.{item.Type}");

            var title = "";
            var description = "";
            var areas = new List<string>();
            var categories = new List<string>();
            var sortiments = new List<string>();
            var tags = new List<string>();
            var itemHasFiletypeWithoutThumbnail = false;

            if (!String.IsNullOrWhiteSpace(item.Title))
                title = item.Title.Trim();

            var titleWithoutSpaces = ToKebabCase(title);
            if (!String.IsNullOrWhiteSpace(item.Description))
                description = item.Description.Trim();

            if (item.Areas != null && item.Areas.Any())
                areas = item.Areas.Split(',').Select(c => c.Trim()).ToList();

            if (item.Categories != null && item.Categories.Any())
                categories = item.Categories.Split(',').Select(c => c.Trim()).ToList();

            if (item.Sortimente != null && item.Sortimente.Any())
                sortiments = item.Sortimente.Split(',').Select(c => c.Trim()).ToList();

            if (item.Tags != null && item.Tags.Any())
                item.Tags.ForEach(tag => tags.Add(tag.Title.Trim()));

            if (item.Type == "doc" || item.Type == "docx" || item.Type == "xls" || item.Type == "xlsx" || item.Type == "zip")
                itemHasFiletypeWithoutThumbnail = true;


            if (downloadFilesFromSharepoint)
            { //DOWNLOADING FILE FROM SHAREPOINT
                var cookieContainer = new CookieContainer();
                using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
                using (var hclient = new HttpClient(handler) { BaseAddress = new Uri(meinapetitoBaseUrl) })
                {
                    cookieContainer.Add(new Uri(meinapetitoBaseUrl), new Cookie("FedAuth", fedAuthToken));
                    var hresponse = await hclient.GetAsync(item.Url);

                    var filePath = $"C:\\json\\output\\{titleWithoutSpaces}.{item.Type}";
                    using (var fs = new FileStream(filePath, FileMode.Create))
                    {
                        await hresponse.Content.CopyToAsync(fs);
                        if (!hresponse.IsSuccessStatusCode)
                            throw new FileNotFoundException();
                    }
                }
                Console.WriteLine("File downloaded successfully");
            }


            if (createThumbnailForDownloads)
            { //CREATE THUMBNAIL
                if (!itemHasFiletypeWithoutThumbnail)
                {
                    var thumbPath = $"C:\\json\\output\\{titleWithoutSpaces}.png";

                    using (Bitmap bmp = ThumbnailExtractor.ExtractThumbnail($"C:\\json\\output\\{titleWithoutSpaces}.{item.Type}", new Size(350, 350), SIIGBF.SIIGBF_RESIZETOFIT))
                    {
                        bmp.Save(thumbPath, ImageFormat.Png);
                    }
                    Console.WriteLine("Thumbnail created successfully");
                }
                else
                {
                    Console.WriteLine("Thumbnail skipped, because filetype is not supported");
                }
            }


            if (createJsonFileForDownloads)
            { //CREATE JSON
                var documentPath = $"{titleWithoutSpaces}.{item.Type}";
                var jsonPath = $"C:\\json\\output\\{titleWithoutSpaces}.json";
                var thumbnailPath = itemHasFiletypeWithoutThumbnail ? null : $"{titleWithoutSpaces}.png";

                var mappedItem = new PrismicUploadDocument
                {
                    Title = CreatePrismicTitleData(title),
                    Description = CreatePrismicDescriptionData(description),
                    Document = new DocumentData
                    {
                        Id = "",
                        Url = documentPath,
                        Name = title,
                        Kind = "document"
                    },
                    Thumbnail = CreatePrismicThumbnailData(thumbnailPath),
                    Categories = CreatePrismicCategoryData(categories),
                    DownloadAreas = CreatePrismicAreaData(areas),
                    DownloadSortiments = CreatePrismicSortimentData(sortiments),
                    Keywords = CreatePrismicKeywordData(tags),
                    Type = "download_dokumente",
                    Tags = tags,
                    Language = languageCode
                };

                var serializerOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                jsonString = JsonSerializer.Serialize(mappedItem, serializerOptions);

                File.WriteAllText(jsonPath, jsonString);
                Console.WriteLine("Json created successfully");
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine("FAILED: " + meinapetitoBaseUrl + item.Url);
            var failedDownload = new Tuple<string, string>(item.Title, item.Url);
            failedDownloads.Add(failedDownload);
            continue;
        }
        finally
        {
            run += 1;
            Console.WriteLine($"File done ({run}/{parsed.DownloadList.Count})");
        }
    }
    Console.WriteLine($"DONE! Catched {failedDownloads.Count} errors.");
}

List<TitleData> CreatePrismicTitleData(string title)
{
    var titelDataList = new List<TitleData>();

    var titel = new TitleData
    {
        Type = "heading1",
        TitleContent = new ContentData
        {
            Text = title,
            Spans = new List<string>()
        }
    };
    titelDataList.Add(titel);
    return titelDataList;
}

List<DescriptionData> CreatePrismicDescriptionData(string description)
{
    var descriptionDataList = new List<DescriptionData>();

    var descriptionData = new DescriptionData
    {
        Type = "paragraph",
        DescriptionContent = new ContentData
        {
            Text = description,
            Spans = new List<string>()
        }
    };
    descriptionDataList.Add(descriptionData);
    return descriptionDataList;
}

List<KeywordData> CreatePrismicKeywordData(List<string> tags)
{
    var keywordList = new List<KeywordData>();
    foreach (var tag in tags)
        keywordList.Add(new KeywordData { Keyword = tag });

    return keywordList;
}

List<CategoryListData> CreatePrismicCategoryData(List<string> categories)
{
    var listData = new List<CategoryListData>();

    foreach (var category in categories)
    {
        var categoriesData = new CategoryListData();
        var mappedCategory = MapCategoryStringToPrismicCategoryId(category);
        if (!string.IsNullOrEmpty(mappedCategory))
        {
            var categoryData = new CategoryData
            {
                Id = mappedCategory,
                WioUrl = $"wio://documents/{mappedCategory}"
            };
            categoriesData.Category = categoryData;
        }
        listData.Add(categoriesData);
    }

    return listData;
}

string MapCategoryStringToPrismicCategoryId(string categoryString)
{
    var prismicStuff = new List<Tuple<string,string>>();
    prismicStuff.Add(new Tuple<string, string>("YeBR6hEAACgAyrGD", "Allergene & Zutaten"));
    prismicStuff.Add(new Tuple<string, string>("YeFyuxEAAC4Az9C_", "Preislisten"));
    prismicStuff.Add(new Tuple<string, string>("YeFy4hEAACgAz9Fe", "Anleitungen & Handbücher"));
    prismicStuff.Add(new Tuple<string, string>("YeFzDxEAACgAz9Hu", "Aktionen"));
    prismicStuff.Add(new Tuple<string, string>("YeKcBBEAACwA1Psd", "Allgemein"));
    prismicStuff.Add(new Tuple<string, string>("YeKcPxEAACwA1Pwx", "Bestellformulare"));
    prismicStuff.Add(new Tuple<string, string>("YeKcaBEAACsA1Pzq", "Bestellungen"));
    prismicStuff.Add(new Tuple<string, string>("YeKchxEAACsA1P1z", "Bilder"));
    prismicStuff.Add(new Tuple<string, string>("YeKcuhEAACgA1P5b", "Ernährungsbildung"));
    prismicStuff.Add(new Tuple<string, string>("YeKdUBEAACsA1QEB", "Ernährungsinformationen"));
    prismicStuff.Add(new Tuple<string, string>("YeKdhREAAC4A1QIC", "HACCP & Qualitätssicherung"));
    prismicStuff.Add(new Tuple<string, string>("YeKdwBEAAC0A1QMM", "Menülisten"));
    prismicStuff.Add(new Tuple<string, string>("YeKeAxEAACwA1QQ0", "mylunch"));
    prismicStuff.Add(new Tuple<string, string>("YeKeLhEAAC0A1QT1", "Pressearbeit"));
    prismicStuff.Add(new Tuple<string, string>("YeKehBEAACsA1QZw", "Sortiment"));
    prismicStuff.Add(new Tuple<string, string>("YeKetxEAAC8A1QdX", "Speisepläne"));
    prismicStuff.Add(new Tuple<string, string>("YeKe8hEAACoA1Qhe", "Vermarktung"));
    prismicStuff.Add(new Tuple<string, string>("YeKfTxEAAC8A1QoZ", "Zubereitung"));
    prismicStuff.Add(new Tuple<string, string>("YeKfLBEAACkA1Qlm", "winVitalis"));
    prismicStuff.Add(new Tuple<string, string>("YfpUVxEAAC8APxkh", "Neue Kategorie für Downloads"));

    var result = prismicStuff.Where(x => x.Item2 == categoryString).Select(y => y.Item1).FirstOrDefault();

    return result ?? "";
}

List<AreaData> CreatePrismicAreaData(List<string> areas)
{
    var areaDataList = new List<AreaData>();
    foreach (var area in areas)
        areaDataList.Add(new AreaData { Area = area });
    return areaDataList;
}

List<SortimentData> CreatePrismicSortimentData(List<string> sortiments)
{
    var sortimentList = new List<SortimentData>();
    foreach (var sortiment in sortiments)
    {
        var sortimentInPrismicFormat = sortiment.ToLower().Replace(' ', '_');
        var sortimentData = new SortimentData { Sortiment = $"meinapetito-development--sortiments~{sortimentInPrismicFormat}" };
        sortimentList.Add(sortimentData);
    }

    return sortimentList;
}

ThumbnailData? CreatePrismicThumbnailData(string? thumbnailPath)
{
    if (string.IsNullOrEmpty(thumbnailPath))
        return null;
    else
    {
        return new ThumbnailData
        {
            Origin = new OriginData
            {
                Id = "",
                Url = thumbnailPath,
            },
            Credits = null,
            AlternateDescription = null,
            Provider = "imgix",
            Thumbnails = new object()
        };
    }
}

string ToKebabCase(string? value)
{
    if (value == null)
    {
        return "";
    }
    value = Regex.Replace(value, "[A-Z][a-z]+", m => $"-{m.ToString().ToLower()}");
    value = Regex.Replace(value, "[A-Z]+", m => $"-{m.ToString().ToLower()}");
    value = Regex.Replace(value, @"[^0-9a-zA-Z]", "-");
    value = Regex.Replace(value, @"[-]{2,}", "-");
    value = Regex.Replace(value, @"-+$", string.Empty);
    if (value.StartsWith("-")) value = value.Substring(1);
    return value.ToLower();
}


Console.ReadKey();




public class MeinapetitoDownload
{
    public List<Download> DownloadList { get; set; }
    public int Count { get; set; }
}

public class Download
{
    public string Title { get; set; }
    public string? Areas { get; set; }
    public string? Categories { get; set; }
    public DateTime? SchedulingStartDate { get; set; }
    public string? Size { get; set; }
    public string? Type { get; set; }
    public string? Url { get; set; }
    public string? Sortimente { get; set; }
    public string? Description { get; set; }
    public List<Tag> Tags { get; set; }

}

public class Tag
{
    public string? Title { get; set; }
    public string? Url { get; set; }
}

public class PrismicUploadDocument
{
    [JsonPropertyName("titel")]
    public List<TitleData> Title { get; set; }

    [JsonPropertyName("beschreibung")]
    public List<DescriptionData> Description { get; set; }

    [JsonPropertyName("dokument")]
    public DocumentData Document { get; set; }

    [JsonPropertyName("vorschaubild")]
    public ThumbnailData? Thumbnail { get; set; }

    [JsonPropertyName("kategorien")]
    public List<CategoryListData> Categories { get; set; }

    [JsonPropertyName("bereiche_fur_download")]
    public List<AreaData> DownloadAreas { get; set; }

    [JsonPropertyName("sortimente_zum_download")]
    public List<SortimentData> DownloadSortiments { get; set; }

    [JsonPropertyName("stichworte")]
    public List<KeywordData> Keywords { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; }

    [JsonPropertyName("lang")]
    public string Language { get; set; }
}

public class TitleData
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("content")]
    public ContentData TitleContent { get; set; }
}

public class DescriptionData
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("content")]
    public ContentData DescriptionContent { get; set; }
}

public class ContentData
{
    [JsonPropertyName("text")]
    public string Text { get; set; }

    [JsonPropertyName("spans")]
    public List<string> Spans { get; set; }
}

public class DocumentData
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("kind")]
    public string Kind { get; set; }
}

public class ThumbnailData
{
    [JsonPropertyName("origin")]
    public OriginData Origin { get; set; }

    [JsonPropertyName("credits")]
    public string? Credits { get; set; }

    [JsonPropertyName("alt")]
    public string? AlternateDescription { get; set; }

    [JsonPropertyName("provider")]
    public string? Provider { get; set; }

    [JsonPropertyName("thumbnails")]
    public object Thumbnails { get; set; }

}

public class OriginData
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }
}

public class CategoryListData
{
    [JsonPropertyName("kategorie")]
    public CategoryData Category { get; set; }
}

public class CategoryData
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("wioUrl")]
    public string WioUrl { get; set; }
}

public class AreaData
{
    [JsonPropertyName("bereich")]
    public string Area { get; set; }
}

public class KeywordData
{
    [JsonPropertyName("stichwort")]
    public string Keyword { get; set; }
}

public class SortimentData
{
    [JsonPropertyName("sortiment")]
    public string Sortiment { get; set; }
}
