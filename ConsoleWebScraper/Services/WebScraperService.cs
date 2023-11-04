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
            var encoding = Encoding.GetEncoding("windows-1251");
            using (var writer = new StreamWriter(fileName, false, encoding))
            {
                HtmlTags.RemoveHtmlTags(writer, text);
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Cannot save file {fileName}: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
        }
    }

    public async Task SaveImagesToDoc(string fileName, string _htmlContent)
    {
        Directory.CreateDirectory(fileName);
        MatchCollection mc = Regex.Matches(_htmlContent, @"<(img)\b[^>]*>");
        List<string> imageURLs = new List<string>();
        foreach (Match m in mc)
        {
            imageURLs.Add(m.ToString());
        }

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
                            Uri uri;
                            if (Uri.TryCreate(part, UriKind.Absolute, out uri))
                            {
                                var response = await client.GetAsync(uri);
                                if (response.IsSuccessStatusCode)
                                {
                                    var bytes = await response.Content.ReadAsByteArrayAsync();
                                    await File.WriteAllBytesAsync($"{fileName}\\Images{pictureNumber}{extension}",
                                        bytes);
                                    pictureNumber++;
                                    Console.WriteLine(part);
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