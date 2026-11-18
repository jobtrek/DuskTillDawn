/* 
Okay so for the email, there are multiple ways to do this. I need to firts generate a bunch of emails and assign each of them a category an id, and a text 

for example the data structure for en email will be 

var byCategory = new Dictionary<Category, List<Email>>();

where the key is the category and then there's the list of emails, and each email has an id.

then when I am querying I can just list all the emails that don't contain the Categories 'Phishing' for example.


The id is for when a user marks an email Phishing then it deletes, it's a way unique way to detect the email.
the categories are for generating the email.


methods to create:

- generates & provides all the emails to the node. then display it
- queries for deleting and marking an email that is Phishing
- Found
- Not Found. 

*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text.Json;
using Godot;
namespace DuskTillDawn.Main.MailUi;

public class Database
{
    public record Email(bool Read, int Id, string Sender, string Subject, string Body, DateTime Date);
    private static int _nextId = 0;

    public enum Categories
    {
        Warning,
        Technews,
        Incident,
        Phishing

    }


    private Dictionary<Categories, List<Email>> byCategory = new();

    // shift starts at 8 o clock
    public static readonly DateTime ShiftStart = new(2026, 11, 12, 20, 0, 0);

    private static readonly DateTime FirstMailTime = new(2026, 11, 12, 22, 0, 0);

    private const int ArrivalJitterMinutes = 10;

    private static readonly Random _rng = new();

    private DateTime _now = ShiftStart;

    public static int generateUniqueId()
    {
        return ++_nextId;

    }

    public Dictionary<Categories, List<Email>> GenerateDatabase()
    {
        byCategory = SeedDatabase();
        return byCategory;
    }


    private const string EmailsPath = "res://main/mail_ui/emails.json";

    private record EmailData(string Sender, string Subject, string Body, int MinutesAfterFirstMail);

    private Dictionary<Categories, List<Email>> SeedDatabase()
    {
        string json = Godot.FileAccess.GetFileAsString(EmailsPath);
        var data = JsonSerializer.Deserialize<Dictionary<Categories, List<EmailData>>>(json)
            ?? throw new InvalidOperationException($"{EmailsPath} is empty or invalid.");

        byCategory = new Dictionary<Categories, List<Email>>();
        foreach (var (category, emails) in data)
        {
            byCategory[category] = emails
                .Select(e => new Email(false, generateUniqueId(), e.Sender, e.Subject, e.Body, Arrival(e.MinutesAfterFirstMail)))
                .ToList();
        }

        return byCategory;
    }


    /// <summary>Authored time (minutes after 22:00) shifted by a random few minutes, so each night plays a bit differently.</summary>
    private static DateTime Arrival(int minutesAfterFirstMail)
    {
        int jitter = _rng.Next(-ArrivalJitterMinutes, ArrivalJitterMinutes + 1);
        return FirstMailTime.AddMinutes(minutesAfterFirstMail + jitter);
    }

    /// <summary>Moves the inbox clock forward. Mails whose time has passed become visible.</summary>
    public void SetTime(DateTime now)
    {
        _now = now;
    }

    /// <summary>Mails that have arrived by the current in-game time.</summary>
    private IEnumerable<Email> Arrived()
    {
        return byCategory.Values
            .SelectMany(emails => emails)
            .Where(email => email.Date <= _now);
    }

    public List<Email> GetAllEmails()
    {
        return Arrived()
            .OrderByDescending(email => email.Date)
            .ToList();
    }

    public int CountAllEmails()
    {
        return Arrived().Count();
    }

    public Email? GetEmail(int id)
    {
        return byCategory.Values
            .SelectMany(emails => emails)
            .FirstOrDefault(email => email.Id == id);
    }
    public bool ReportPhishing(int id)
    {
        if (!byCategory.TryGetValue(Categories.Phishing, out var phishing))
        {
            return false;
        }

        int removed = phishing.RemoveAll(email => email.Id == id);
        return removed > 0;
    }

    public void MarkRead(int id)
    {
        foreach (var emails in byCategory.Values)
        {
            int index = emails.FindIndex(email => email.Id == id);
            if (index >= 0)
            {
                emails[index] = emails[index] with { Read = true };
                return;
            }
        }
    }

    public int UnReadCount()
    {
        return Arrived().Count(email => !email.Read);
    }


}