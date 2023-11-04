namespace ConsoleWebScraper.Interfaces;

public interface IWebScraperService
{
    Task SaveUrlsToDoc(string fileName, List<string> innerUrls);
    Task SaveContentToDoc(string fileName, string text);
    Task SaveImagesToDoc(string fileName, string _htmlContent);
}