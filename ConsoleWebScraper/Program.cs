// See https://aka.ms/new-console-template for more information


using System.Text;
using ConsoleWebScraper;
using ConsoleWebScraper.Commands;
using ConsoleWebScraper.Presentation;
using Ninject;
using Ninject.Modules;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
NinjectModule serviceModule = new ServiceModule();
var kernel = new StandardKernel(serviceModule);

var controller = new Controller(kernel);
var client = new Client(controller);
client.Start();