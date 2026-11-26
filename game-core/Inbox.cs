using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace DuskTillDawn.Core;

public sealed class Inbox
{
    // The schedule file counts from the first mail at 22:00, two hours into the shift.
    private const int FirstMailMinutesAfterNightStart = 120;
    // Each night shifts every mail by a few minutes so two nights never play exactly the same.
    private const int ArrivalJitterMinutes = 10;
    private static readonly TimeOnly NightStart = new(20, 0);

    private readonly List<Mail> _mails = new();
    private readonly List<(int Minute, Mail Mail)> _schedule = new();
    private int _nextId = 1;

    // In arrival order, oldest first.
    public IReadOnlyList<Mail> Mails => _mails;

    public int UnreadCount => _mails.Count(mail => !mail.IsRead);

    public void LoadSchedule(string json, Random? random = null)
    {
        random ??= Random.Shared;
        var data = JsonSerializer.Deserialize<Dictionary<MailKind, List<ScheduledMail>>>(json)
            ?? throw new ArgumentException("The mail schedule is empty or invalid.", nameof(json));

        _nextId = 1;
        var scheduled = new List<(int Minute, Mail Mail)>();
        foreach (var (kind, mails) in data)
        {
            foreach (ScheduledMail entry in mails)
            {
                int minute = FirstMailMinutesAfterNightStart + entry.MinutesAfterFirstMail
                    + random.Next(-ArrivalJitterMinutes, ArrivalJitterMinutes + 1);
                scheduled.Add((minute, new Mail
                {
                    Id = _nextId++,
                    Kind = kind,
                    From = entry.From,
                    Subject = entry.Subject,
                    Body = entry.Body,
                    ArrivedAt = NightStart.AddMinutes(minute),
                }));
            }
        }

        _mails.Clear();
        _schedule.Clear();
        _schedule.AddRange(scheduled.OrderBy(item => item.Minute));
    }

    public void MarkRead(int mailId)
    {
        Get(mailId).IsRead = true;
    }

    internal Mail Get(int mailId)
    {
        return _mails.FirstOrDefault(mail => mail.Id == mailId)
            ?? throw new ArgumentException($"No mail {mailId} in the inbox.", nameof(mailId));
    }

    // A correctly reported Phishing mail is deleted; any other reported mail stays.
    internal void MarkReported(Mail mail)
    {
        mail.IsReported = true;
        if (mail.Kind == MailKind.Phishing)
        {
            _mails.Remove(mail);
        }
    }

    internal Mail? DeliverWarning(Challenge incident, TimeOnly arrivedAt)
    {
        if (incident.Warning is not { } text)
        {
            return null;
        }

        var mail = new Mail
        {
            Id = _nextId++,
            Kind = MailKind.Warning,
            From = text.From,
            Subject = text.Subject,
            Body = text.Body,
            ChallengeId = incident.Id,
            ArrivedAt = arrivedAt,
        };
        _mails.Add(mail);
        return mail;
    }

    internal IEnumerable<Mail> DeliverDue(double minutesSinceNightStart)
    {
        while (_schedule.Count > 0 && _schedule[0].Minute <= minutesSinceNightStart)
        {
            Mail mail = _schedule[0].Mail;
            _schedule.RemoveAt(0);
            _mails.Add(mail);
            yield return mail;
        }
    }

    // The next scheduled mail of this kind arrives now instead of at its planned time.
    internal Mail? DeliverNext(MailKind kind)
    {
        int index = _schedule.FindIndex(item => item.Mail.Kind == kind);
        if (index < 0)
        {
            return null;
        }

        Mail mail = _schedule[index].Mail;
        _schedule.RemoveAt(index);
        _mails.Add(mail);
        return mail;
    }

    private sealed record ScheduledMail(string From, string Subject, string Body, int MinutesAfterFirstMail);
}
