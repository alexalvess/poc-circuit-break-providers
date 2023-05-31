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

Console.WriteLine("Start Application");

while (true)
{
    var key = Console.ReadLine();

    if (key == "0")
    {
        for (int i = 0; i < 3; i++)
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.WriteLine($"Round {i + 1}");
            Console.ResetColor();
            Console.WriteLine("WCF Proxy result: " + string.Join(',', await Task.WhenAll(CreateThreads(6))));
        }
    }
    else
        break;
}

List<Task<int>> CreateThreads(int numberOfThreads)
{

    List<Task<int>> threads = new();
    for (int i = 0; i < numberOfThreads; i++)
    {
        var proxy = builder.GetRequiredService<ICalculatorProxy>();
        threads.Add(proxy.AddAsync(2, 2));
    }

    return threads;
}

Console.WriteLine("End Application");
Console.ReadKey();