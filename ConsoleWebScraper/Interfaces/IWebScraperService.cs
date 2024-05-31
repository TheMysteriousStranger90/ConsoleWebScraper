namespace ConsoleWebScraper.Interfaces;

public interface IWebScraperService
{
    Task SaveUrlsToDoc(string fileName, List<string> innerUrls);
    Task SaveContentToDoc(string fileName, string htmlContent);
    Task SaveImagesToDoc(string fileName, string htmlContent, string baseUrl);
}