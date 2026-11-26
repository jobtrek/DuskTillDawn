using System;
using System.Collections.Generic;

namespace DuskTillDawn.Core;

public enum LadderStage
{
    Ready,
    Playing,
    StepWon,
    Lost,
    Completed,
}

public enum Difficulty
{
    Easy,
    Medium,
    Hard,
}

public sealed record LadderStep(string Name, Difficulty Difficulty, Func<Challenge> Start);

public sealed class ChallengeLadder
{
    public const double BreakSeconds = 4;

    private readonly IReadOnlyList<LadderStep> _steps;
    private double _breakLeft;

    public ChallengeLadder(IReadOnlyList<LadderStep> steps)
    {
        if (steps.Count == 0)
        {
            throw new ArgumentException("Give at least one step.", nameof(steps));
        }

        _steps = steps;
    }

    public LadderStage Stage { get; private set; } = LadderStage.Ready;
    public int StepIndex { get; private set; }
    public int StepCount => _steps.Count;
    public LadderStep Step => _steps[StepIndex];
    public Challenge? Current { get; private set; }

    public event Action<LadderStage>? StageChanged;
    public static ChallengeLadder Demo(Game game, PhishingIncidents phishing)
    {
        return new ChallengeLadder(
        [
            new LadderStep("Fire in the server room", Difficulty.Easy, () => Launched(game, new Fire(durationSeconds: 90))),
            new LadderStep("Traffic attack on the servers", Difficulty.Medium,
                () => Launched(game, new SecurityAttack(durationSeconds: 60))),
            new LadderStep("Phishing mail", Difficulty.Hard, () => phishing.SendNow(durationSeconds: 60)
                ?? throw new InvalidOperationException("No Phishing mail left in the schedule.")),
        ]);
    }

    public void Start()
    {
        if (Stage is LadderStage.Ready or LadderStage.Lost)
        {
            StartStep(0);
        }
    }

    // Called after Game.Tick, so a challenge that just failed or was fixed is seen in the same frame.
    public void Tick(double seconds)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(seconds);

        if (Stage == LadderStage.Playing)
        {
            CheckCurrent();
        }
        else if (Stage == LadderStage.StepWon)
        {
            _breakLeft -= seconds;
            if (_breakLeft <= 0)
            {
                StartStep(StepIndex + 1);
            }
        }
    }

    private void CheckCurrent()
    {
        if (Current!.State == ChallengeState.Failed)
        {
            ChangeStage(LadderStage.Lost);
        }
        else if (Current.State == ChallengeState.Succeeded)
        {
            bool lastStep = StepIndex == _steps.Count - 1;
            _breakLeft = BreakSeconds;
            ChangeStage(lastStep ? LadderStage.Completed : LadderStage.StepWon);
        }
    }

    private void StartStep(int index)
    {
        StepIndex = index;
        Current = _steps[index].Start();
        ChangeStage(LadderStage.Playing);
    }

    private void ChangeStage(LadderStage stage)
    {
        Stage = stage;
        StageChanged?.Invoke(stage);
    }

    private static Challenge Launched(Game game, Challenge challenge)
    {
        game.Launch(challenge);
        return challenge;
    }
}
