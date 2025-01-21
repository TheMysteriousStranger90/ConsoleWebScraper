using System.Net;
using System.Text;
using ConsoleWebScraper.Helpers;
using ConsoleWebScraper.Interfaces;
using ConsoleWebScraper.Logging;

namespace ConsoleWebScraper.Services;

public class WebScraperService : IWebScraperService
{
    private readonly SemaphoreSlim _throttler = new(5);
    private readonly HttpClient _httpClient;

    public WebScraperService()
    {
        var handler = new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };

        _httpClient = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromSeconds(30)
        };
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 WebScraper/1.0");
    }

    public async Task SaveUrlsToDoc(string? fileName, List<string> innerUrls)
    {
        try
        {
            var validUrls = innerUrls
                .Where(url => Uri.TryCreate(url, UriKind.Absolute, out _))
                .Distinct()
                .ToList();

            await File.WriteAllLinesAsync(fileName, validUrls);
            Logger.Log($"Saved {validUrls.Count} URLs to {fileName}", LogLevel.Success);
        }
        catch (Exception ex)
        {
            Logger.Log($"Error saving URLs: {ex.Message}", LogLevel.Error);
            throw;
        }
    }

    public async Task SaveContentToDoc(string? fileName, string htmlContent)
    {
        try
        {
            await using var writer = new StreamWriter(fileName, false, Encoding.UTF8);
            await HtmlTags.RemoveHtmlTagsAsync(writer, htmlContent);
            Logger.Log($"Content saved to {fileName}", LogLevel.Success);
        }
        catch (Exception ex)
        {
            Logger.Log($"Error saving content: {ex.Message}", LogLevel.Error);
            throw;
        }
    }

    public async Task SaveImagesToDoc(string? fileName, string htmlContent, string baseUrl)
    {
        var doc = new HtmlAgilityPack.HtmlDocument();
        doc.LoadHtml(htmlContent);

        var imageNodes = doc.DocumentNode.SelectNodes("//img");
        if (imageNodes == null) return;

        var baseUri = new Uri(baseUrl);
        var tasks = new List<Task>();

        foreach (var imgNode in imageNodes)
        {
            var src = imgNode.GetAttributeValue("src", null);
            if (string.IsNullOrEmpty(src)) continue;

            try
            {
                var imageUrl = new Uri(baseUri, src);
                tasks.Add(DownloadImageAsync(imageUrl, fileName));
            }
            catch (Exception ex)
            {
                Logger.Log($"Invalid image URL {src}: {ex.Message}", LogLevel.Warning);
            }
        }

        await Task.WhenAll(tasks);
    }

    private async Task DownloadImageAsync(Uri imageUrl, string? savePath)
    {
        await _throttler.WaitAsync();
        try
        {
            var fileName = Path.GetFileName(imageUrl.LocalPath);
            var filePath = Path.Combine(savePath, fileName);

            var response = await _httpClient.GetAsync(imageUrl);
            if (!response.IsSuccessStatusCode)
            {
                Logger.Log($"Failed to download {imageUrl}: {response.StatusCode}", LogLevel.Warning);
                return;
            }

            await using var fs = new FileStream(filePath, FileMode.Create);
            await response.Content.CopyToAsync(fs);
            Logger.Log($"Downloaded {fileName}", LogLevel.Success);
        }
        catch (Exception ex)
        {
            Logger.Log($"Error downloading {imageUrl}: {ex.Message}", LogLevel.Error);
        }
        finally
        {
            _throttler.Release();
        }
    }
}