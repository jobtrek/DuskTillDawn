using System.Collections.Generic;

namespace DuskTillDawn.Core.Servers;

public sealed class Server
{
    public string Name { get; }
    public IReadOnlyList<string> Clients { get; }
    public double Load { get; } = 0.5;
    public ServerHealth Status { get; internal set; } = ServerHealth.Ok;

    public Server(string name, params string[] clients)
    {
        Name = name;
        Clients = clients;
    }
}
