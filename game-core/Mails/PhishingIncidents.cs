using System;
using System.Collections.Generic;
using DuskTillDawn.Core.Incidents;
using DuskTillDawn.Core.Night;

namespace DuskTillDawn.Core.Mails;

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

    private void OnMailReceived(Mail mail)
    {
        if (mail.Kind != MailKind.Phishing || _launched >= _maxPerNight || _game.IsRunning<PhishingEmail>())
        {
            return;
        }

        var incident = new PhishingEmail();
        _byMail[mail.Id] = incident;
        _launched++;
        _game.Launch(incident);
    }
}
