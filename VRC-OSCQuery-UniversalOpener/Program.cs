using VRC.OSCQuery;
using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;

class Program
{
    private static OSCQueryService? server;
    void Main(string[] args)
    {
        AppDomain.CurrentDomain.ProcessExit += new EventHandler(Currentdomain_ProcessExit);
        dynamic config = getConfig();
        var tcpport = Extensions.GetAvailableTcpPort();
        var udpport = Extensions.GetAvailableUdpPort();
        server = new OSCQueryServiceBuilder()
            .WithDefaults()
            .WithTcpPort(tcpport)
            .WithUdpPort(udpport)
            .WithServiceName(config.name)
            .Build();

        foreach (var endpoint in config.endpoints)
        {
            server.AddEndpoint((string)endpoint, "s", Attributes.AccessValues.ReadWrite, null, $"Endpoint {(string)endpoint} created.");
        }

        config.tcpport = tcpport;
        config.udpport = udpport;
        string output = JsonConvert.SerializeObject(config, Formatting.Indented);
        File.WriteAllText("config.json", output);
        Console.WriteLine("OSCQuery Service Opened");

        Console.ReadKey();
        server.Dispose();

    }

    private dynamic getConfig()
    {
        string path = "config.json";
        dynamic? obj = JsonConvert.DeserializeObject(path);
        
        if (obj == null)
        {
            obj = new
            {
                name = "VRC OSCQuery Service",
                endpoints = new string[] { "/avatar/parameters/'tis-not-working" },
                tcpport = 0,
                udpport = 0
            };
        }
        return obj;
    }

    static void Currentdomain_ProcessExit(object? sender, EventArgs e)
    {
        // Cleanup code here
        if (server != null)
        {
            server.Dispose();
            server = null;
        }
    }   
}