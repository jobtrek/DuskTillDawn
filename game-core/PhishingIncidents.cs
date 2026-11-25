using System.Collections.Generic;

namespace DuskTillDawn.Core;

// What the player gets back from a report: whether the mail was Phishing, and the points it earned or cost.
public readonly record struct PhishingReport(bool IsPhishing, int Points);

// Every Phishing mail is an Incident: it starts when the mail arrives and is fixed by reporting that mail.
public sealed class PhishingIncidents
{
    private readonly GameState _state;
    private readonly Game _game;
    private readonly Dictionary<int, PhishingEmail> _byMail = [];

    public PhishingIncidents(GameState state, Game game)
    {
        _state = state;
        _game = game;
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
        if (mail.Kind != MailKind.Phishing)
        {
            return;
        }

        var incident = new PhishingEmail();
        _byMail[mail.Id] = incident;
        _game.Launch(incident);
    }
}
