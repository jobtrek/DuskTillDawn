using System.Collections.Generic;
using System.Globalization;
using System.Text;
using DuskTillDawn.Core.Incidents;
using DuskTillDawn.Core.Power;

namespace DuskTillDawn.Core.Night;

// What happened during one night, for balancing playtests: a sample every half in-game hour,
// then the result. Many logs together give the win rate per night and the SLA left at 08:00.
public sealed class NightLog
{
    private const double SampleEveryInGameMinutes = 30;

    private readonly List<string> _rows = [];
    private double _nextSampleMinute;

    public void Sample(GameState state)
    {
        if (state.MinutesSinceNightStart < _nextSampleMinute)
        {
            return;
        }

        _nextSampleMinute += SampleEveryInGameMinutes;
        ServerRoom room = state.ServerRoom;
        _rows.Add(string.Join(',',
            state.ClockText,
            Number(state.Sla),
            Number(room.Heat),
            Number(room.Battery),
            Number(room.Traffic),
            room.Status,
            room.EcoMode));
    }

    public string ToCsv(GameState state, Game game)
    {
        int fixedCount = 0;
        int failedCount = 0;
        foreach (Incident incident in game.Incidents)
        {
            if (incident.State == IncidentState.Succeeded)
            {
                fixedCount++;
            }
            else if (incident.State == IncidentState.Failed)
            {
                failedCount++;
            }
        }

        bool won = state.Status == NightStatus.Won && !game.IsLost;
        var csv = new StringBuilder();
        csv.AppendLine("result,sla_left,fixed,failed,points,ended_at");
        csv.AppendLine(string.Join(',', won ? "won" : "lost", Number(state.Sla), fixedCount, failedCount, state.Points, state.ClockText));
        csv.AppendLine();
        csv.AppendLine("clock,sla,heat,battery,traffic,servers,eco_mode");
        foreach (string row in _rows)
        {
            csv.AppendLine(row);
        }

        return csv.ToString();
    }

    private static string Number(double value)
    {
        return value.ToString("0.0", CultureInfo.InvariantCulture);
    }
}
