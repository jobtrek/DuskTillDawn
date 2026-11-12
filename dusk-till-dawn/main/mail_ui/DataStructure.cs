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

    public static int generateUniqueId()
    {
        return ++_nextId;

    }

    public Dictionary<Categories, List<Email>> GenerateDatabase()
    {
        byCategory = SeedDatabase();
        return byCategory;
    }


    private Dictionary<Categories, List<Email>> SeedDatabase()
    {
        var night = new DateTime(2026, 11, 12, 22, 0, 0);

        byCategory = new Dictionary<Categories, List<Email>>
        {
            [Categories.Warning] = new List<Email>
            {
                new(false, generateUniqueId(), "IT Security", "Disk almost full on DB-01",
                    "DB-01 is at 92% disk usage. Clear old logs before it hits 100% and the database stops.",
                    night.AddMinutes(5)),
                new(false, generateUniqueId(), "Monitoring", "High CPU on WEB-02",
                    "WEB-02 has been above 90% CPU for 10 minutes. Check for a runaway process.",
                    night.AddMinutes(20)),
                new(false, generateUniqueId(), "Monitoring", "Memory leak suspected on APP-03",
                    "APP-03 RAM usage grows 2% every 10 minutes. Restart the service before it runs out of memory.",
                    night.AddMinutes(70)),
                new(false, generateUniqueId(), "IT Security", "SSL certificate expires in 3 days",
                    "The certificate for shop.dusk-corp.local expires on Sunday. Renew it or customers will see a security error.",
                    night.AddMinutes(95)),
                new(false, generateUniqueId(), "Monitoring", "Packet loss on SW-CORE-01",
                    "The core switch shows 4% packet loss on port 12. Check the cable or the port.",
                    night.AddMinutes(130)),
                new(false, generateUniqueId(), "Monitoring", "UPS battery at 60%",
                    "The UPS in rack B switched to battery after a power dip. If power does not return, servers shut down in 25 minutes.",
                    night.AddMinutes(170)),
                new(false, generateUniqueId(), "IT Security", "5 failed logins on admin account",
                    "Someone tried to log in as 'admin' 5 times from 10.0.4.77. Check if it was a colleague or lock the account.",
                    night.AddMinutes(210)),
                new(false, generateUniqueId(), "Monitoring", "Server room temperature rising",
                    "Server room is at 29°C. One cooling unit may have stopped. Check the AC before servers overheat.",
                    night.AddMinutes(265)),
                new(false, generateUniqueId(), "Monitoring", "Slow response on DB-01",
                    "Queries on DB-01 take 4 seconds instead of 0.2. Look for a locked table or a heavy query.",
                    night.AddMinutes(320)),
                new(false, generateUniqueId(), "IT Security", "Antivirus definitions out of date",
                    "12 workstations have not updated their antivirus in 7 days. Push the update from the admin console.",
                    night.AddMinutes(400)),
            },
            [Categories.Technews] = new List<Email>
            {
                new(false, generateUniqueId(), "Tech News", "Log4Shell: patch now",
                    "A critical flaw in Log4j lets attackers run code on your servers. Update to the latest version.",
                    night.AddMinutes(2)),
                new(false, generateUniqueId(), "Tech News", "Why backups fail when you need them",
                    "Most teams never test a restore. Schedule one this week.",
                    night.AddMinutes(45)),
                new(false, generateUniqueId(), "Tech News", "How to spot a phishing mail",
                    "Check the sender address letter by letter. Real IT teams never ask for your password by mail.",
                    night.AddMinutes(80)),
                new(false, generateUniqueId(), "Tech News", "Ransomware hits 3 hospitals",
                    "Attackers got in through an unpatched VPN. All three had no offline backup.",
                    night.AddMinutes(115)),
                new(false, generateUniqueId(), "Tech News", "The 3-2-1 backup rule",
                    "3 copies of your data, on 2 different media, 1 of them off-site. Simple and still the best.",
                    night.AddMinutes(155)),
                new(false, generateUniqueId(), "Tech News", "Why your server room needs a thermometer",
                    "Most hardware failures in small companies start with heat. A 10 € sensor can save a server.",
                    night.AddMinutes(200)),
                new(false, generateUniqueId(), "Tech News", "MFA blocks 99% of account attacks",
                    "Turning on two-factor login is the cheapest security upgrade you can make.",
                    night.AddMinutes(250)),
                new(false, generateUniqueId(), "Tech News", "CEO fraud costs companies millions",
                    "Scammers pose as the boss and ask for urgent payments or gift cards. Always call to check.",
                    night.AddMinutes(300)),
                new(false, generateUniqueId(), "Tech News", "Night shift tips for sysadmins",
                    "Drink water, check alerts every 15 minutes, and write down everything you change.",
                    night.AddMinutes(360)),
                new(false, generateUniqueId(), "Tech News", "Old disks: when to replace them",
                    "Watch the SMART reallocated sector count. If it keeps rising, the disk is dying.",
                    night.AddMinutes(430)),
            },
            [Categories.Incident] = new List<Email>
            {
                new(false, generateUniqueId(), "Admin Team", "Mail server down",
                    "Customers cannot send mail. MAIL-01 is not responding. Restart it from the admin console.",
                    night.AddMinutes(30)),
                new(false, generateUniqueId(), "Admin Team", "Backup job failed",
                    "Last night's backup did not finish. Run it again before morning.",
                    night.AddMinutes(60)),
                new(false, generateUniqueId(), "Admin Team", "Website is down",
                    "The shop shows 'Error 502'. WEB-01 stopped answering. Restart the web service.",
                    night.AddMinutes(100)),
                new(false, generateUniqueId(), "Helpdesk", "Printer queue stuck on floor 2",
                    "The morning shift can't print. Clear the print queue on PRINT-02.",
                    night.AddMinutes(140)),
                new(false, generateUniqueId(), "Admin Team", "Database is full",
                    "DB-01 hit 100% disk. Orders are failing. Free space now, then restart the database.",
                    night.AddMinutes(185)),
                new(false, generateUniqueId(), "Admin Team", "VPN is down",
                    "Remote staff cannot connect. VPN-01 is not answering. Restart the VPN service.",
                    night.AddMinutes(230)),
                new(false, generateUniqueId(), "Helpdesk", "User locked out",
                    "Sarah from Accounting is locked out after too many wrong passwords. Unlock her account.",
                    night.AddMinutes(280)),
                new(false, generateUniqueId(), "Admin Team", "Network switch offline",
                    "Floor 3 has no network. SW-03 lost power. Check the PDU in rack C.",
                    night.AddMinutes(340)),
                new(false, generateUniqueId(), "Admin Team", "Server overheating",
                    "APP-02 shut down at 85°C. Fix the cooling, then power it back on.",
                    night.AddMinutes(390)),
                new(false, generateUniqueId(), "Admin Team", "DNS not resolving",
                    "Nobody can reach internal sites by name. DNS-01 crashed. Restart the DNS service.",
                    night.AddMinutes(450)),
            },
            [Categories.Phishing] = new List<Email>
            {
                new(false, generateUniqueId(), "IT Securty", "Reset your password NOW",
                    "Your account will be deleted in 1 hour. Click here and enter your password to keep it.",
                    night.AddMinutes(15)),
                new(false, generateUniqueId(), "CEO (personal)", "Urgent gift cards",
                    "I'm in a meeting. Buy 5 gift cards and send me the codes. Don't tell anyone.",
                    night.AddMinutes(50)),
                new(false, generateUniqueId(), "Micros0ft Support", "Your license has expired",
                    "Your Office license expired. Pay 49.99 € in the next 2 hours at micros0ft-renew.com or lose all files.",
                    night.AddMinutes(90)),
                new(false, generateUniqueId(), "HR Departement", "Salary update for 2027",
                    "Your new salary is ready. Open the attached file salary_2027.pdf.exe and enable macros to view it.",
                    night.AddMinutes(125)),
                new(false, generateUniqueId(), "DHL Express", "Your parcel is waiting",
                    "We could not deliver your parcel. Pay 1.99 € customs fee here to get it delivered tomorrow.",
                    night.AddMinutes(160)),
                new(false, generateUniqueId(), "Admin Teamm", "Mailbox full",
                    "Your mailbox is 99% full. Log in here with your username and password to get 50 GB free.",
                    night.AddMinutes(220)),
                new(false, generateUniqueId(), "Bank Security", "Suspicious payment blocked",
                    "We blocked a payment of 2,450 €. Confirm your card number and PIN to unblock your account.",
                    night.AddMinutes(275)),
                new(false, generateUniqueId(), "CEO (personal)", "Wire transfer today",
                    "Send 18,000 € to the account below before 8am. It's for a secret deal, don't call me, I'm on a plane.",
                    night.AddMinutes(330)),
                new(false, generateUniqueId(), "IT-Helpdesk", "New VPN client required",
                    "Install the new VPN client from vpn-update.ru today or you will lose remote access.",
                    night.AddMinutes(380)),
                new(false, generateUniqueId(), "Lottery Team", "You won 1,000,000 €!",
                    "Congratulations! Send your name, address and bank details to claim your prize.",
                    night.AddMinutes(440)),
            },
        };



        return byCategory;
    }


    // for this query I need a way so make it progressive and random
    public List<Email> GetAllEmails()
    {
        return byCategory.Values
            .SelectMany(emails => emails)
            .OrderByDescending(email => email.Date)
            .ToList();
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
        return byCategory.Values
            .SelectMany(emails => emails)
            .Count(email => !email.Read);
    }


}