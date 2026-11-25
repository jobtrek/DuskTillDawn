namespace DuskTillDawn.Core;

public sealed class Server
{
    public string Name { get; }
    public int Clients { get; }
    public double Load { get; } = 0.5;
    public ServerStatus Status { get; internal set; } = ServerStatus.Ok;

    public Server(string name, int clients)
    {
        Name = name;
        Clients = clients;

    }




}
