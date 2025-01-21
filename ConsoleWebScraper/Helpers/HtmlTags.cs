namespace ConsoleWebScraper.Helpers;

public static class HtmlTags
{
    public static void RemoveHtmlTags(StreamWriter writer, string htmlContent)
    {
        var doc = new HtmlAgilityPack.HtmlDocument();
        doc.LoadHtml(htmlContent);

        var scriptNodes = doc.DocumentNode.DescendantsAndSelf().Where(n => n.Name == "script");
        foreach (var scriptNode in scriptNodes.ToList())
        {
            scriptNode.Remove();
        }

        var jsonLdNodes = doc.DocumentNode.DescendantsAndSelf()
            .Where(n => n.GetAttributeValue("type", "") == "application/ld+json");
        foreach (var jsonLdNode in jsonLdNodes.ToList())
        {
            jsonLdNode.Remove();
        }

        string textOnly = doc.DocumentNode.InnerText;
        string trimmed = textOnly.Trim();

        if (!string.IsNullOrEmpty(trimmed))
        {
            writer.WriteLine(trimmed);
        }
    }
}