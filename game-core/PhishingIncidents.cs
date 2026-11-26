using System;
using System.Collections.Generic;

namespace DuskTillDawn.Core;

// What the player gets back from a report: whether the mail was Phishing, and the points it earned or cost.
public readonly record struct PhishingReport(bool IsPhishing, int Points);

// A Phishing mail can be an Incident: it starts when the mail arrives and is fixed by reporting that mail.
// Only one runs at a time and the night allows a few; the other Phishing mails are only reported, for no points.
public sealed class PhishingIncidents
{
    private readonly GameState _state;
    private readonly Game _game;
    private readonly Dictionary<int, PhishingEmail> _byMail = [];
    private readonly int _maxPerNight;
    private int _launched;
    private double? _forcedDuration;

    public PhishingIncidents(GameState state, Game game, int maxPerNight)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maxPerNight);
        _state = state;
        _game = game;
        _maxPerNight = maxPerNight;
        _state.MailReceived += OnMailReceived;
    }

    // A wrong report costs points, a Phishing mail reported in time repairs its Incident.
    public PhishingReport Report(int mailId)
    {
        int pointsBefore = _state.Points;
        bool isPhishing = _state.ReportPhishing(mailId);
        if (isPhishing && _byMail.Remove(mailId, out PhishingEmail? incident))
        {
            incident.Repair(_state.Player, PlayerAction.ReportEmail);
        }

        return new PhishingReport(isPhishing, _state.Points - pointsBefore);
    }

    // The next Phishing mail arrives now and is always an Incident, whatever the night allows.
    public PhishingEmail? SendNow(double durationSeconds)
    {
        _forcedDuration = durationSeconds;
        Mail? mail = _state.ReceiveNow(MailKind.Phishing);
        _forcedDuration = null;
        return mail != null && _byMail.TryGetValue(mail.Id, out PhishingEmail? incident) ? incident : null;
    }

    private void OnMailReceived(Mail mail)
    {
        if (_forcedDuration is { } forced && mail.Kind == MailKind.Phishing)
        {
            Launch(mail, new PhishingEmail(forced));
            return;
        }

        if (mail.Kind != MailKind.Phishing || _launched >= _maxPerNight || _game.IsRunning<PhishingEmail>())
        {
            return;
        }

        Launch(mail, new PhishingEmail());
    }

    private void Launch(Mail mail, PhishingEmail incident)
    {
        _byMail[mail.Id] = incident;
        _launched++;
        _game.Launch(incident);
    }
}
