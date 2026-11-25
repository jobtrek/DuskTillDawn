using System;

namespace DuskTillDawn.Core;

// How hard a night is (docs/game-v1.md §9): which challenges can happen and how often a new one starts.
// Each night adds challenges and starts them faster; night 5 is the final night with all six.
public sealed record NightPlan(double SpawnIntervalSeconds, Func<Challenge>[] Kinds)
{
    public const int FirstNight = 1;
    public const int LastNight = 5;

    public static NightPlan For(int night, ServerRack servers)
    {
        return night switch
        {
            1 => new NightPlan(180, [() => new BrokenDisk(servers), () => new SecurityAttack(), () => new OverheatingServer()]),
            2 => new NightPlan(150,
            [
                () => new BrokenDisk(servers), () => new SecurityAttack(), () => new OverheatingServer(), () => new PhishingEmail(),
            ]),
            3 => new NightPlan(120,
            [
                () => new BrokenDisk(servers), () => new SecurityAttack(), () => new OverheatingServer(),
                () => new PhishingEmail(), () => new UnpluggedCable(),
            ]),
            4 => new NightPlan(90, AllSix(servers)),
            5 => new NightPlan(75, AllSix(servers)),
            _ => throw new ArgumentOutOfRangeException(nameof(night), night, $"Nights go from {FirstNight} to {LastNight}."),
        };
    }

    private static Func<Challenge>[] AllSix(ServerRack servers)
    {
        return
        [
            () => new BrokenDisk(servers), () => new SecurityAttack(), () => new PhishingEmail(),
            () => new UnpluggedCable(), () => new OverheatingServer(), () => new Fire(),
        ];
    }
}
