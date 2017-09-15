using System;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus;

class Program
{
    static void Main()
    {
        AsyncMain().GetAwaiter().GetResult();
    }

    static async Task AsyncMain()
    {
        Console.Title = "Samples.DataDistribution.Subscriber-2";

        const string endpointName = "Samples.DataDistribution.Subscriber-2";

        var endpointConfiguration = MainConfig(endpointName);
        var mainEndpoint = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

        //var endpointConfiguration = new EndpointConfiguration("Samples.DataDistribution.Subscriber");
        //endpointConfiguration.UsePersistence<InMemoryPersistence>();
        //endpointConfiguration.EnableInstallers();
        //endpointConfiguration.SendFailedMessagesTo("error");

        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
        await mainEndpoint.Stop().ConfigureAwait(false);
    }

    static EndpointConfiguration MainConfig(string endpointName)
    {
        #region MainConfig

        var mainConfig = new EndpointConfiguration(endpointName);

        var typesToExclude = AllTypes
            .Where(t => t.Namespace == "DataDistribution").ToArray();

        var scanner = mainConfig.AssemblyScanner();
        scanner.ExcludeTypes(typesToExclude);
        var transport = mainConfig.UseTransport<MsmqTransport>();
        var mainRouting = transport.Routing();
        mainRouting.RegisterPublisher(
            publisherEndpoint: "Samples.DataDistribution.Server-2",
            eventType: typeof(OrderAccepted));

        #endregion

        ApplyDefaults(mainConfig);
        return mainConfig;
    }

    static Type[] AllTypes => typeof(Program).Assembly.GetTypes();

    static void ApplyDefaults(EndpointConfiguration endpointConfiguration)
    {
        endpointConfiguration.UsePersistence<InMemoryPersistence>();
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.SendFailedMessagesTo("error");
    }
}