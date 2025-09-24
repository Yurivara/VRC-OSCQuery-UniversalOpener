using VRC.OSCQuery;
using Newtonsoft.Json;
using System.Runtime.InteropServices;

class Program
{
    public class Config
    {
        [JsonProperty("endpoints")]
        public string[]? endpoints { get; set; }

        [JsonProperty("name")]
        public string? name { get; set; }

        [JsonProperty("user_app_path")]
        public string? user_app_path { get; set; }

        [JsonProperty("tcpport")]
        public int tcpport { get; set; }

        [JsonProperty("udpport")]
        public int udpport { get; set; }
    }
    private static OSCQueryService? server;
    private static string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
    static void Main(string[] args)
    {
        AppDomain.CurrentDomain.ProcessExit += new EventHandler(Currentdomain_ProcessExit);
        var config = getConfig();
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
        File.WriteAllText(configPath, output);
        Console.WriteLine("OSCQuery Service Opened");

        Console.ReadKey();
        server.Dispose();
    }

    private static Config getConfig()
    {
        string json = File.ReadAllText(configPath);
        var result = JsonConvert.DeserializeObject<Config>(json);
        if (result == null)
            throw new InvalidOperationException("Failed to deserialize config.json.");
        return result;
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


/*
 * Created by Yurivara on GitHub.
 * https://www.cup0tea.online
 * 
 * This falls under the BSD-3-Clause License.
 */