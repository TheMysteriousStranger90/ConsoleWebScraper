namespace ConsoleWebScraper.Helpers;

public static class HtmlTags
{
    public static async Task RemoveHtmlTagsAsync(StreamWriter writer, string htmlContent)
    {
        var doc = new HtmlAgilityPack.HtmlDocument();
        doc.LoadHtml(htmlContent);

        // Remove unwanted nodes
        var nodesToRemove = doc.DocumentNode.SelectNodes("//script|//style|//meta|//link|//comment()");
        nodesToRemove?.ToList().ForEach(node => node.Remove());
        
        var textNodes = doc.DocumentNode
            .DescendantsAndSelf()
            .Where(n => !n.HasChildNodes && !string.IsNullOrWhiteSpace(n.InnerText))
            .Select(node => node.InnerText.Trim())
            .Where(text => text.Length > 1)
            .Select(NormalizeText);
        
        foreach (var text in textNodes)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                await writer.WriteLineAsync(text);
            }
        }
    }

    private static string NormalizeText(string text)
    {
        return text
            .Replace("&nbsp;", " ")
            .Replace("\t", " ")
            .Replace("\r", "")
            .Replace("\n", " ")
            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
            .Aggregate((a, b) => a + " " + b)
            .Trim();
    }
}