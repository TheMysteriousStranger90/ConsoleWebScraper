﻿using System.Net;
using System.Text.RegularExpressions;
using ConsoleWebScraper.Interfaces;
using ConsoleWebScraper.Services;
using Ninject;

namespace ConsoleWebScraper.Presentation;

public class Controller
{
    private readonly IWebScraperService _webScraperService;
    private HttpClient _client;
    private string _htmlContent { get; set; }
    private string _url { get; set; }
    public List<string> _innerURLs { get; set; }

    private static string path1;
    private static string path2;
    private static string path3;

    public Controller(IKernel kernel)
    {
        _webScraperService = kernel.Get<WebScraperService>();
        _client = new HttpClient();
        _innerURLs = new List<string>();
    }

    public string PleaseEnterAValidUrl()
    {
        CreateSpecialFolder();
        Console.WriteLine("Please enter a valid URL: ");
        Console.WriteLine("\n");

        _url = Console.ReadLine();

        try
        {
            HttpResponseMessage response = _client.GetAsync(_url).Result;
            _htmlContent = response.Content.ReadAsStringAsync().Result;
            Regex regExpression = new Regex("(?:href)=[\"|']?(.*?)[\"|'|>]+",
                RegexOptions.Singleline | RegexOptions.CultureInvariant);
            if (regExpression.IsMatch(_htmlContent))
            {
                foreach (Match match in regExpression.Matches(_htmlContent))
                {
                    string matchValue = match.Groups[1].Value;
                    _innerURLs.Add(matchValue);
                }
            }
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine("The URL you entered is not valid. Please try again.");
            PleaseEnterAValidUrl();
        }

        _webScraperService.SaveUrlsToDoc(path1, _innerURLs);
        _webScraperService.SaveContentToDoc(path2, _htmlContent);
        _webScraperService.SaveImagesToDoc(path3, _htmlContent, _url);

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
    }

    public string Exit()
    {
        Environment.Exit(0);
        return "Exiting the application...";
    }
}