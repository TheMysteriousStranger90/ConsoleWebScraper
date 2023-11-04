using ConsoleWebScraper.Interfaces;
using ConsoleWebScraper.Services;
using Ninject.Modules;

namespace ConsoleWebScraper;

internal class ServiceModule : NinjectModule
{
    public override void Load()
    {
        Bind<IWebScraperService>().To<WebScraperService>();
    }
}