using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using ConsoleWebScraper.Helpers;
using ConsoleWebScraper.Interfaces;

namespace ConsoleWebScraper.Services;

public class WebScraperService : IWebScraperService
{
    public async Task SaveUrlsToDoc(string fileName, List<string> innerUrls)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            throw new ArgumentException("Output file name cannot be null or empty.", nameof(fileName));
        }

        try
        {
            using (var writer = new StreamWriter(fileName))
            {
                foreach (var url in innerUrls)
                {
                    await writer.WriteLineAsync(url);
                }
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine(
                $"An error occurred while saving the file: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
        }
    }

    public async Task SaveContentToDoc(string fileName, string text)
    {
        try
        {
            using (var writer = new StreamWriter(fileName, false, Encoding.UTF8))
            {
                HtmlTags.RemoveHtmlTags(writer, text);
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Cannot save file {fileName}: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
        }
    }

    public async Task SaveImagesToDoc(string fileName, string _htmlContent, string baseUrl)
    {
        Directory.CreateDirectory(fileName);

        var doc = new HtmlAgilityPack.HtmlDocument();
        doc.LoadHtml(_htmlContent);

        var imageNodes = doc.DocumentNode.DescendantsAndSelf().Where(n => n.Name == "img");
        List<string> imageURLs = imageNodes.Select(n => n.GetAttributeValue("src", "")).ToList();

        int pictureNumber = 1;

        string[] extensions = { ".jpg", ".png", ".svg" };

        using (HttpClient client = new HttpClient())
        {
            foreach (var item in imageURLs)
            {
                string[] split = item.Split(new Char[] { '"', '?' });
                foreach (var part in split)
                {
                    foreach (var extension in extensions)
                    {
                        if (part.Contains(extension))
                        {
                            string absoluteUrl = part.StartsWith("http") ? part : baseUrl + part;

                            Uri uri;
                            if (Uri.TryCreate(absoluteUrl, UriKind.Absolute, out uri))
                            {
                                var response = await client.GetAsync(uri);
                                if (response.IsSuccessStatusCode)
                                {
                                    var bytes = await response.Content.ReadAsByteArrayAsync();
                                    await File.WriteAllBytesAsync($"{fileName}\\Images{pictureNumber}{extension}",
                                        bytes);
                                    pictureNumber++;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}