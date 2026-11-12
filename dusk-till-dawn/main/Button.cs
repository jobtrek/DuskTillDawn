using Godot;
using System;
using DuskTillDawn.Main.MailUi;
public partial class Button : Godot.Button
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Pressed += OnPressed;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

    }

    private void OnPressed()
    {
        new Database().GenerateDatabase();
    }
}
