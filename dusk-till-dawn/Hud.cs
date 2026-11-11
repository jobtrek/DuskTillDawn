using DuskTillDawn.Core;
using Godot;

public partial class Hud : CanvasLayer
{
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
        _endLabel.Visible = state.Status != NightStatus.Running;
        _endLabel.Text = state.Status == NightStatus.Won
            ? "You survived the night"
            : "SLA at 0 %: the company is bankrupt";
    }
}
