using System.Text.RegularExpressions;

namespace ConsoleWebScraper.Helpers;

public static class HtmlTags
{
    public static void RemoveHtmlTags(StreamWriter writer, string text)
    {
        string htmlTagPattern = "<.*?>";
        string whitespacePattern = @"\n\s+";

        var regexHtml = new Regex(htmlTagPattern);
        var regexWhitespace = new Regex(whitespacePattern);

        string noTags = regexHtml.Replace(text, string.Empty);
        string trimmed = noTags.Trim();
        string textOnly = regexWhitespace.Replace(trimmed, "\n");

        if (!string.IsNullOrEmpty(textOnly))
        {
            writer.WriteLine(textOnly);
        }
    }
}