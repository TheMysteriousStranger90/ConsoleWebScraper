using System.Text.RegularExpressions;

namespace ConsoleWebScraper.Helpers;

public static class HtmlTags
{
    public static void RemoveHtmlTags(StreamWriter writer, string htmlContent)
    {
        string htmlTagPattern = "<.*?>";
        string whitespacePattern = @"\n\s+";

        var regexHtml = new Regex(htmlTagPattern);
        var regexWhitespace = new Regex(whitespacePattern);

        string noTags = regexHtml.Replace(htmlContent, string.Empty);
        string trimmed = noTags.Trim();
        string textOnly = regexWhitespace.Replace(trimmed, "\n");

        if (!string.IsNullOrEmpty(textOnly))
        {
            writer.WriteLine(textOnly);
        }
    }
}