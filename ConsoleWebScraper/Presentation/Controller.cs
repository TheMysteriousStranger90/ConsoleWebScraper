using System.Net;
using System.Text.RegularExpressions;
using ConsoleWebScraper.Interfaces;
using ConsoleWebScraper.Logging;
using ConsoleWebScraper.Services;
using Ninject;

namespace ConsoleWebScraper.Presentation;

public class Controller
{
    private readonly IWebScraperService _webScraperService;
    private readonly HttpClient _client;
    private string HtmlContent { get; set; }
    private string Url { get; set; }
    private List<string> _innerURLs { get; set; }

    private static string? path1;
    private static string? path2;
    private static string? path3;

    public Controller(IKernel kernel)
    {
        _webScraperService = kernel.Get<WebScraperService>();
        _client = new HttpClient();
        _innerURLs = new List<string>();
    }

    public async Task<string> PleaseEnterAValidUrl()
    {
        CreateSpecialFolder();
        Console.WriteLine("Please enter a valid URL: ");
        Console.WriteLine("\n");

        Url = Console.ReadLine();

        try
        {
            HttpResponseMessage response = await _client.GetAsync(Url);
            HtmlContent = await response.Content.ReadAsStringAsync();
            Regex regExpression = new Regex("(?:href)=[\"|']?(.*?)[\"|'|>]+",
                RegexOptions.Singleline | RegexOptions.CultureInvariant);
            if (regExpression.IsMatch(HtmlContent))
            {
                foreach (Match match in regExpression.Matches(HtmlContent))
                {
                    string matchValue = match.Groups[1].Value;
                    _innerURLs.Add(matchValue);
                }
            }
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine("The URL you entered is not valid. Please try again.");
            return await PleaseEnterAValidUrl();
        }

        await _webScraperService.SaveUrlsToDoc(path1, _innerURLs);
        await _webScraperService.SaveContentToDoc(path2, HtmlContent);
        await _webScraperService.SaveImagesToDoc(path3, HtmlContent, Url);
        
        await Task.Delay(1000);
        Console.Clear();
    
        return "Created";
    }

    public void CreateSpecialFolder()
    {
        var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        var projectFolder = Path.Combine(desktop, "WebScrapperProject");

        if (!Directory.Exists(projectFolder))
        {
            Directory.CreateDirectory(projectFolder);
        }

        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        var timestampFolder = Path.Combine(projectFolder, timestamp);

        Directory.CreateDirectory(timestampFolder);

        path1 = Path.Combine(timestampFolder, "List.txt");
        path2 = Path.Combine(timestampFolder, "Content.txt");
        path3 = Path.Combine(timestampFolder, "Images");

        Directory.CreateDirectory(path3);
        
        Logger.Initialize(timestampFolder);
        Logger.Log("Web scraper session started", LogLevel.Info);
    }

    public string Exit()
    {
        Environment.Exit(0);
        return "Exiting the application...";
    }
}