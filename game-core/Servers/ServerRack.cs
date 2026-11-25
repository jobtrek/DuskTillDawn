using System;
using System.Collections.Generic;

namespace DuskTillDawn.Core;

public sealed class ServerRack
{
    private readonly List<Server> _servers = [];
    private readonly Random _random;

    public ServerRack(Random? random = null)
    {
        _random = random ?? new Random();
        _servers.Add(new Server("WEB-01", "Shop", "Bakery Rossi", "Hotel Lac", "Florist", "Garage Meier", "Library"));
        _servers.Add(new Server("WEB-02", "Town Hall", "School", "Dentist", "Law Office"));
        _servers.Add(new Server("DB-01", "Shop", "Clinic Nord", "Pharmacy", "Hotel Lac", "Town Hall", "Garage Meier", "Dentist", "Law Office"));
        _servers.Add(new Server("MAIL-01", "Clinic Nord", "School", "Bakery Rossi", "Town Hall", "Law Office"));
        _servers.Add(new Server("APP-02", "Clinic Nord", "Pharmacy", "Dentist"));
        _servers.Add(new Server("APP-03", "Shop", "Hotel Lac", "Library"));
        _servers.Add(new Server("DNS-01", "Town Hall", "School"));
        _servers.Add(new Server("VPN-01", "Clinic Nord"));
    }

    public IReadOnlyList<Server> All => _servers;


    public Server PickRandom()
    {
        return _servers[_random.Next(_servers.Count)];
    }



}
