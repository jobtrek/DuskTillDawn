using System;

namespace DuskTillDawn.Core;

// How hard a night is (docs/game-v1.md §9): which challenges can happen and how often a new one starts.
// Each night adds challenges and starts them faster; night 5 is the final night with all of them.
// Fake e-mails are not spawned here: they come from Phishing mails (PhishingIncidents), at most FakeEmails per night.
public sealed record NightPlan(double SpawnIntervalSeconds, int FakeEmails, Func<Challenge>[] Kinds)
{
    public const int FirstNight = 1;
    public const int LastNight = 5;

    public static NightPlan For(int night, ServerRack servers)
    {
        return night switch
        {
            1 => new NightPlan(180, 1, [() => new BrokenDisk(servers), () => new SecurityAttack(), () => new OverheatingServer()]),
            2 => new NightPlan(150, 2, [() => new BrokenDisk(servers), () => new SecurityAttack(), () => new OverheatingServer()]),
            3 => new NightPlan(120, 2,
            [
                () => new BrokenDisk(servers), () => new SecurityAttack(), () => new OverheatingServer(), () => new UnpluggedCable(),
            ]),
            4 => new NightPlan(90, 3, AllKinds(servers)),
            5 => new NightPlan(75, 3, AllKinds(servers)),
            _ => throw new ArgumentOutOfRangeException(nameof(night), night, $"Nights go from {FirstNight} to {LastNight}."),
        };
    }

    private static Func<Challenge>[] AllKinds(ServerRack servers)
    {
        return
        [
            () => new BrokenDisk(servers), () => new SecurityAttack(),
            () => new UnpluggedCable(), () => new OverheatingServer(), () => new ElectricalFault(),
        ];
    }
}
