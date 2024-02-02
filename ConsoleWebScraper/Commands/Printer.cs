namespace ConsoleWebScraper.Commands;

public static class Printer
{
    public static void StartPage()
    {
        Console.ForegroundColor = ConsoleColor.Green;

        string title = @"
 ##   ##           ###       #####
 ##   ##            ##      ##   ##
 ##   ##   ####     ##      #         ####    ######    ####    ######    ####    ######
 ## # ##  ##  ##    #####    #####   ##  ##    ##  ##      ##    ##  ##  ##  ##    ##  ##
 #######  ######    ##  ##       ##  ##        ##       #####    ##  ##  ######    ##
 ### ###  ##        ##  ##  ##   ##  ##  ##    ##      ##  ##    #####   ##        ##
 ##   ##   #####   ######    #####    ####    ####      #####    ##       #####   ####
                                                                ####
                                                      
";
        Console.WriteLine(title);

        string text = "Menu guide:\n";
        Console.WriteLine(text);

        Console.ResetColor();
    }

    public static void MainMenu()
    {
        Console.ForegroundColor = ConsoleColor.White;
        
        string menu = "\n1 --> Activate command\n";
        menu += "0 --> Quit\n";
        Console.WriteLine(menu);
    }
}