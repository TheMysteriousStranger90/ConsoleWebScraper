using ConsoleWebScraper.Presentation;

namespace ConsoleWebScraper.Commands;

public class Client
{
    private delegate string Command();

    private Dictionary<string, Command> commands;
    private readonly Controller _controller;
    private bool quit = false;

    public Client(Controller controller)
    {
        _controller = controller;
    }

    private void ChooseCommand()
    {
        var commandKey = Console.ReadKey(true);

        if (commandKey.Key == ConsoleKey.NumPad0)
        {
            quit = true;
            return;
        }

        if (commands.TryGetValue(commandKey.KeyChar.ToString(), out Command command))
        {
            Console.WriteLine(command());
        }
        else
            Console.WriteLine("\nUnknown command.\n");
    }

    private void MainPresentation()
    {
        Printer.MainMenu();
        commands = new Dictionary<string, Command>()
        {
            { "1", _controller.PleaseEnterAValidUrl },
            { "0", _controller.Exit },
        };
        ChooseCommand();
    }

    public void Start()
    {
        Printer.StartPage();
        while (!quit)
        {
            MainPresentation();
        }
    }
}