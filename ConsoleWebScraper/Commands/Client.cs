using ConsoleWebScraper.Presentation;

namespace ConsoleWebScraper.Commands;

public class Client
{
    private delegate Task<string> Command();

    private Dictionary<string, Command> _commands;
    private readonly Controller _controller;
    private bool _quit = false;

    public Client(Controller controller)
    {
        _controller = controller;
    }

    private async Task ChooseCommand()
    {
        var commandKey = Console.ReadKey(true);

        if (commandKey.Key == ConsoleKey.NumPad0)
        {
            _quit = true;
            return;
        }

        if (_commands.TryGetValue(commandKey.KeyChar.ToString(), out Command command))
        {
            Console.WriteLine(await command());
        }
        else
            Console.WriteLine("\nUnknown command.\n");
    }

    private void MainPresentation()
    {
        Printer.MainMenu();
        _commands = new Dictionary<string, Command>()
        {
            { "1", async () => await _controller.PleaseEnterAValidUrl() },
            { "0", () => Task.FromResult(_controller.Exit()) }
        };
        ChooseCommand().Wait();
    }

    public void Start()
    {
        Printer.StartPage();
        while (!_quit)
        {
            MainPresentation();
        }
    }
}