using System;
using System.Collections.Generic;

namespace DuskTillDawn.Core;

public sealed class ServerRoom
{
    private readonly List<Server> _servers = [];
    private readonly Random _random = new();

    public ServerRoom()
    {
        _servers.Add(new Server("WEB-01", 6));
        _servers.Add(new Server("WEB-02", 4));
        _servers.Add(new Server("DB-01", 8));
        _servers.Add(new Server("MAIL-01", 5));
        _servers.Add(new Server("APP-02", 3));
        _servers.Add(new Server("APP-03", 3));
        _servers.Add(new Server("DNS-01", 2));
        _servers.Add(new Server("VPN-01", 1));
    }

    public IReadOnlyList<Server> All => _servers;


    public Server PickRandom()
    {
        return _servers[_random.Next(_servers.Count)];
    }



}
