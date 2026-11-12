using System.Collections.Generic;
using Godot;
using static DuskTillDawn.Main.MailUi.Database;

namespace DuskTillDawn.Main.MailUi;

/// <summary>
/// Mail window shown inside the desk computer screen (spec: docs/specs/mail_app_spec.md).
/// Inbox on the left (40 %), reading pane on the right (60 %).
///
/// The UI only shows mails and emits signals. <see cref="MailController"/> owns the data
/// and decides what a report means, then calls <see cref="SetMails"/> / <see cref="ShowReportResult"/>.
/// </summary>
public partial class MailApp : Control
{
    [Signal] public delegate void MailOpenedEventHandler(int id);
    [Signal] public delegate void ReportPressedEventHandler(int id);
    [Signal] public delegate void CloseRequestedEventHandler();

    [Export] public float OpenDuration = 0.25f;
    [Export] public float RevealDuration = 0.5f;

    private static readonly Color RowNormal = new(0f, 0f, 0f, 0f);
    private static readonly Color RowHover = new(1f, 1f, 1f, 0.06f);
    private static readonly Color RowSelected = new(0.32f, 0.47f, 0.75f, 0.35f);
    private static readonly Color Good = new(0.45f, 0.85f, 0.45f);
    private static readonly Color Bad = new(0.95f, 0.4f, 0.35f);

    private VBoxContainer _rows = null!;
    private Control _reader = null!;
    private Label _empty = null!;
    private Label _from = null!;
    private Label _subject = null!;
    private Label _date = null!;
    private Label _body = null!;
    private Label _result = null!;
    private Godot.Button _report = null!;
    private BlockReveal _reveal = null!;
    private FontVariation _bold = null!;
    private Tween? _openTween;

    private Email? _selected;
    // Mail id → was the report right? Kept by the UI so the button stays disabled.
    private readonly Dictionary<int, bool> _reports = new();

    public override void _Ready()
    {
        _rows = GetNode<VBoxContainer>("Body/Inbox/Rows");
        _reader = GetNode<Control>("Body/Reader/Mail");
        _empty = GetNode<Label>("Body/Reader/Empty");
        _from = GetNode<Label>("Body/Reader/Mail/From");
        _subject = GetNode<Label>("Body/Reader/Mail/Subject");
        _date = GetNode<Label>("Body/Reader/Mail/Date");
        _body = GetNode<Label>("Body/Reader/Mail/BodyText");
        _result = GetNode<Label>("Body/Reader/Mail/Footer/Result");
        _report = GetNode<Godot.Button>("Body/Reader/Mail/Footer/Report");
        _reveal = GetNode<BlockReveal>("BlockReveal");
        _bold = new FontVariation { BaseFont = GetThemeFont("font", "Label"), VariationEmbolden = 0.9f };

        var close = GetNode<Godot.Button>("TitleBar/Close");
        close.Pressed += () => EmitSignal(SignalName.CloseRequested);
        WireHover(close);

        _report.Pressed += OnReportPressed;
        WireHover(_report);

        ShowReader(null);

        Resized += () => PivotOffset = Size / 2f;
        VisibilityChanged += () =>
        {
            if (IsVisibleInTree())
            {
                PlayOpen();
            }
        };
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (IsVisibleInTree() && @event.IsActionPressed("ui_cancel"))
        {
            GetViewport().SetInputAsHandled();
            EmitSignal(SignalName.CloseRequested);
        }
    }

    /// <summary>Rebuilds the inbox. Mails should already be sorted newest first.</summary>
    public void SetMails(IEnumerable<Email> mails)
    {
        foreach (var child in _rows.GetChildren())
        {
            child.QueueFree();
        }

        foreach (var mail in mails)
        {
            _rows.AddChild(MakeRow(mail));
        }
    }

    /// <summary>Called by the game after a ReportPressed, with whether the mail really was phishing.</summary>
    public void ShowReportResult(int id, bool correct)
    {
        _reports[id] = correct;
        if (_selected?.Id == id)
        {
            ShowReader(_selected);
        }
    }

    private Godot.Button MakeRow(Email mail)
    {
        bool selected = _selected?.Id == mail.Id;
        var fill = selected ? RowSelected : RowNormal;

        var row = new Godot.Button
        {
            CustomMinimumSize = new Vector2(0, 30),
            FocusMode = FocusModeEnum.None,
            ClipContents = true,
        };
        row.AddThemeStyleboxOverride("normal", Box(fill));
        row.AddThemeStyleboxOverride("pressed", Box(fill));
        row.AddThemeStyleboxOverride("focus", new StyleBoxEmpty());
        row.AddThemeStyleboxOverride("hover", Box(selected ? RowSelected : RowHover));
        row.Pressed += () => Select(mail);

        var line = new HBoxContainer { MouseFilter = MouseFilterEnum.Ignore };
        line.AddThemeConstantOverride("separation", 8);
        row.AddChild(line);
        line.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        line.OffsetLeft = 8;
        line.OffsetRight = -8;

        line.AddChild(MakeLabel(mail.Read ? "" : "●", 12, 14));
        line.AddChild(MakeLabel(mail.Sender, 13, 120));
        var subject = MakeLabel(mail.Subject, 13, 0);
        subject.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        if (!mail.Read)
        {
            subject.AddThemeFontOverride("font", _bold);
        }
        else
        {
            subject.Modulate = new Color(1f, 1f, 1f, 0.55f);
        }
        line.AddChild(subject);

        return row;
    }

    private static Label MakeLabel(string text, int size, float minWidth)
    {
        var label = new Label
        {
            Text = text,
            CustomMinimumSize = new Vector2(minWidth, 0),
            VerticalAlignment = VerticalAlignment.Center,
            TextOverrunBehavior = TextServer.OverrunBehavior.TrimEllipsis,
            ClipText = true,
            MouseFilter = MouseFilterEnum.Ignore,
        };
        label.AddThemeFontSizeOverride("font_size", size);
        return label;
    }

    private static StyleBoxFlat Box(Color color)
    {
        return new StyleBoxFlat { BgColor = color };
    }

    private void Select(Email mail)
    {
        _selected = mail;
        ShowReader(mail);
        // The game marks it read and calls SetMails again, which redraws the selection.
        EmitSignal(SignalName.MailOpened, mail.Id);
    }

    private void ShowReader(Email? mail)
    {
        _empty.Visible = mail == null;
        _reader.Visible = mail != null;
        if (mail == null)
        {
            return;
        }

        _from.Text = $"From: {mail.Sender}";
        _subject.Text = mail.Subject;
        _date.Text = mail.Date.ToString("HH:mm");
        _body.Text = mail.Body;

        bool reported = _reports.TryGetValue(mail.Id, out bool correct);
        _report.Disabled = reported;
        _result.Text = !reported ? "" : correct ? "Reported ✓" : "Not phishing";
        _result.Modulate = correct ? Good : Bad;
    }

    private void OnReportPressed()
    {
        if (_selected == null || _reports.ContainsKey(_selected.Id))
        {
            return;
        }
        _report.Disabled = true;
        EmitSignal(SignalName.ReportPressed, _selected.Id);
    }

    private static void WireHover(Godot.Button button)
    {
        button.MouseEntered += () => SetHover(button, true);
        button.MouseExited += () => SetHover(button, false);
    }

    private static void SetHover(Godot.Button button, bool hovered)
    {
        button.PivotOffset = button.Size / 2f;
        var tween = button.CreateTween().SetParallel();
        tween.TweenProperty(button, "scale", Vector2.One * (hovered ? 1.12f : 1f), 0.08);
        tween.TweenProperty(button, "self_modulate", hovered ? new Color(1.25f, 1.25f, 1.25f) : Colors.White, 0.08);
    }

    private void PlayOpen()
    {
        _openTween?.Kill();

        Scale = Vector2.One * 0.85f;
        Modulate = Colors.Transparent;
        _reveal.Progress = 0f;

        _openTween = CreateTween().SetParallel();
        _openTween.TweenProperty(this, "modulate", Colors.White, OpenDuration * 0.6f);
        _openTween.TweenProperty(this, "scale", Vector2.One, OpenDuration)
            .SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
        _openTween.TweenProperty(_reveal, "Progress", 1f, RevealDuration)
            .SetDelay(OpenDuration * 0.4f);
    }
}
