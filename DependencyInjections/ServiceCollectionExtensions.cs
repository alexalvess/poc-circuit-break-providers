using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using poc_circuit_break_providers.DependencyInjections.Options;
using poc_circuit_break_providers.Services;
using poc_circuit_break_providers.Services.WebServices;
using poc_circuit_break_providers.Services.WebServices.Proxies;
using Polly;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;

namespace poc_circuit_break_providers.DependencyInjections;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebServicesDependency(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<CalculatorWebServiceOptions>()
            .Bind(configuration.GetSection(nameof(CalculatorWebServiceOptions)));

        services.AddTransient(provider =>
        {
            var options = provider.GetRequiredService<IOptionsMonitor<CalculatorWebServiceOptions>>().CurrentValue;

            BasicHttpBinding binding = new()
            {
                MaxBufferSize = int.MaxValue,
                ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max,
                MaxReceivedMessageSize = int.MaxValue,
                AllowCookies = options.AllowCookies,
                SendTimeout = options.SendTimeout,
                ReceiveTimeout = options.ReceiveTimeout
            };

            EndpointAddress endpoint = new(options.Endpoint);

            var channel = new ChannelFactory<ICalculatorService>(binding, endpoint);

            return channel.CreateChannel();
        });

        services.AddTransient<ICalculatorProxy, CalculatorProxy>();
        services.AddSingleton<WcfControlPolicy>();

        return services;
    }
}
