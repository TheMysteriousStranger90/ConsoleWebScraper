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

        var images = doc.DocumentNode.Descendants("img")
            .Select(e => e.GetAttributeValue("src", null))
            .Where(src => !string.IsNullOrEmpty(src))
            .Select(src => new Uri(new Uri(baseUrl), src).AbsoluteUri)
            .ToList();

        using (HttpClient client = new HttpClient())
        {
            int pictureNumber = 1;
            foreach (var img in images)
            {
                try
                {
                    var imageBytes = await client.GetByteArrayAsync(img);
                    var extension = Path.GetExtension(new Uri(img).AbsolutePath);
                    await File.WriteAllBytesAsync($"{fileName}\\Images{pictureNumber}{extension}", imageBytes);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to download or save image {img}: {ex.Message}");
                }
                pictureNumber++;
            }
        }
    }
}