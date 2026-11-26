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

// Start launches the step's Incident and returns it, or null when it cannot happen (the step is then skipped).
public sealed record LadderStep(string Name, Difficulty Difficulty, Func<Challenge?> Start);

public sealed class ChallengeLadder
{
    private readonly IReadOnlyList<LadderStep> _steps;

    public ChallengeLadder(Game game, IReadOnlyList<LadderStep> steps)
    {
        if (steps.Count == 0)
        {
            throw new ArgumentException("Give at least one step.", nameof(steps));
        }

        _steps = steps;
        game.ChallengeSucceeded += OnSucceeded;
        game.ChallengeFailed += OnFailed;
    }

    public LadderStage Stage { get; private set; } = LadderStage.Ready;
    public int StepIndex { get; private set; }
    public int StepCount => _steps.Count;
    public LadderStep Step => _steps[StepIndex];
    public Challenge? Current { get; private set; }

    public event Action<LadderStage>? StageChanged;

    public static ChallengeLadder Demo(Game game, PhishingIncidents phishing)
    {
        return new ChallengeLadder(game,
        [
            new LadderStep("Fire in the server room", Difficulty.Easy, () => Launched(game, new Fire(durationSeconds: 90))),
            new LadderStep("Traffic attack on the servers", Difficulty.Medium,
                () => Launched(game, new SecurityAttack(durationSeconds: 75))),
            new LadderStep("Phishing mail", Difficulty.Hard, () => phishing.SendNow(durationSeconds: 60)),
        ]);
    }

    public void Start()
    {
        if (Stage is LadderStage.Ready or LadderStage.Lost)
        {
            StartStep(0);
        }
    }

    // The player gets a moment to see the step was won; the UI decides how long, then moves on.
    public void Continue()
    {
        if (Stage == LadderStage.StepWon)
        {
            StartStep(StepIndex + 1);
        }
    }

    private void OnSucceeded(Challenge challenge)
    {
        if (Stage != LadderStage.Playing || challenge != Current)
        {
            return;
        }

        bool lastStep = StepIndex == _steps.Count - 1;
        ChangeStage(lastStep ? LadderStage.Completed : LadderStage.StepWon);
    }

    private void OnFailed(Challenge challenge)
    {
        if (Stage == LadderStage.Playing && challenge == Current)
        {
            ChangeStage(LadderStage.Lost);
        }
    }

    private void StartStep(int index)
    {
        StepIndex = index;
        Current = _steps[index].Start();
        if (Current == null)
        {
            ChangeStage(index == _steps.Count - 1 ? LadderStage.Completed : LadderStage.StepWon);
            return;
        }

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
