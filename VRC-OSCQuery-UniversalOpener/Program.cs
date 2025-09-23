using VRC.OSCQuery;
using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;

class Program
{
    void Main(string[] args)
    {
        dynamic config = getConfig();
        var tcpport = Extensions.GetAvailableTcpPort();
        var udpport = Extensions.GetAvailableUdpPort();
        var server = new OSCQueryServiceBuilder()
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

    }

    private dynamic getConfig()
    {
        string path = "config.json";
        dynamic obj = JsonConvert.DeserializeObject(path);
        
        return obj;
    }
}