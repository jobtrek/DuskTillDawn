using DuskTillDawn.Core;
using Godot;

public partial class Hud : CanvasLayer
{
	private const double NightSurvivedBannerSeconds = 5;

	private Label _clockLabel = null!;
	private Label _slaLabel = null!;
	private Label _endLabel = null!;

	public override void _Ready()
	{
		_clockLabel = GetNode<Label>("ClockLabel");
		_slaLabel = GetNode<Label>("SlaLabel");
		_endLabel = GetNode<Label>("EndLabel");
	}

	public void ShowState(GameState state)
	{
		_clockLabel.Text = state.ClockText;
		_slaLabel.Text = $"SLA {state.Sla:0.0} %";
		string? message = EndMessage(state);
		_endLabel.Visible = message != null;
		_endLabel.Text = message ?? "";
	}

	private static string? EndMessage(GameState state)
	{
		if (state.Status == NightStatus.Lost)
		{
			return "SLA at 0 %: the company is bankrupt";
		}

		if (state.DaySurvived)
		{
			return "Day 1 survived";
		}

		// Shown for a few seconds at 08:00, then the day goes on without the banner.
		return state.Status == NightStatus.Won && state.RealSecondsSinceNightEnded < NightSurvivedBannerSeconds
			? "You survived the night"
			: null;
	}
}
