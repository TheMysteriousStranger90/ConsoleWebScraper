namespace ConsoleWebScraper.Commands;

public static class Printer
{
    public static void StartPage()
    {
        string text = "Welcome to Web Scraper!\n\n";
        text += "Menu guide:\n";
        Console.WriteLine(text);
    }

    public static void MainMenu()
    {
        string menu = "\n1 --> Activate command\n";
        menu += "0 --> Quit\n";
        Console.WriteLine(menu);
    }
}