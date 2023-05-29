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
        for (int i = 0; i < 3; i++)
        {
            Console.WriteLine($"Rodada {i + 1}");
            Console.WriteLine("WCF Proxy result: " + string.Join(',', await Task.WhenAll(CreateThreads(wcfProxy, 6))));
            Thread.Sleep(TimeSpan.FromSeconds(3));
        }
    }
    else
        break;
}

List<Task<int>> CreateThreads(ICalculatorProxy proxy, int numberOfThreads)
{
    List<Task<int>> threads = new();
    for (int i = 0; i < numberOfThreads; i++)
        threads.Add(proxy.AddAsync(2, 2));

    return threads;
}

Console.WriteLine("End Application");
Console.ReadKey();