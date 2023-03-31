using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using poc_circuit_break_providers.DependencyInjections;
using poc_circuit_break_providers.Services.WebServices;

IConfigurationRoot configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
    .AddJsonFile("appsettings.json", false)
    .Build();

var serviceCollections = new ServiceCollection()
    .AddWebServicesDependency(configuration);

var builder = serviceCollections.BuildServiceProvider();

var wcfProxy = builder.GetRequiredService<ICalculatorProxy>();

Console.WriteLine("Start Application");

while (true)
{
    var key = Console.ReadLine();

    if (key == "0")
    {
        Console.WriteLine("WCF Proxy result: " + await wcfProxy.AddAsync(2, 2));
    }
    else
        break;
}

Console.WriteLine("End Application");
Console.ReadKey();