/* 
so for the monitor service I need to stock these 5 things
┌─────────┬────────────────────────────────┬─────────────────────────┬─────────────────────────────────┐
│  Value  │             Range              │ Orange (close to limit) │        Red (past limit)         │
├─────────┼────────────────────────────────┼─────────────────────────┼─────────────────────────────────┤
│ Power   │ 0–100 %                        │ not decided yet         │ 0 % = servers offline           │
├─────────┼────────────────────────────────┼─────────────────────────┼─────────────────────────────────┤
│ Heat    │ 20–100 °C                      │ not decided yet         │ 80 °C = fire risk, 95 °C = fire │
├─────────┼────────────────────────────────┼─────────────────────────┼─────────────────────────────────┤
│ Traffic │ 0–100 % (about 40 % is normal) │ not decided yet         │ 100 % = requests dropped        │
├─────────┼────────────────────────────────┼─────────────────────────┼─────────────────────────────────┤
│ Disk    │ 0–100 %                        │ not decided yet         │ 100 % = crash                   │
├─────────┼────────────────────────────────┼─────────────────────────┼─────────────────────────────────┤
│ SLA     │ 0–100 %                        │ not decided yet         │ 0 % = game lost                 │
└─────────┴────────────────────────────────┴─────────────────────────┴─────────────────────────────────┘

The range are a bunch of numbers so I can just use a Dictionary with Type, and Values to store these. 
then for the queries I can just do queries to fetch all of those values, make the methods public and update the state in GameState

to calculate the thing between green, orange and red. I need to use linear interpolation which is a way to calculate the distance between
2 data points across a linear relationship.

t = (value - Good) / (Bad - Good)    // 0 at Good, 1 at Bad, clamp to [0,1]

t is basically how far have we traveled from good toward bad.


pGood = Power
hGood = Heat
tGood = Traffic
dGood = Disk
sGood = SLA

pBad = Power
hBad = Heat
tBad = Traffic
dBad = Disk
sBad = SLA

t = (value - pGood) / (pBad - pGood)


clamping means basically limiting a value to a specifc range, between a declared minimum and a maximum value.


Lerp and inverse lerp are the same line, read in opposite directions:

  Inverse lerp : value -> t      t     = (value - Good) / (Bad - Good)
  Lerp         : t -> value      value = Good + (Bad - Good) * t

  Godot: Mathf.InverseLerp(Good, Bad, value)   and   Mathf.Lerp(Good, Bad, t)

Ruler (Heat):
  Good = 20                          value = 84        Bad = 95
  |----------------------------------------|---------------|
  t = 0                                  t = 0.85        t = 1


INVERSE LERP (value -> t)
┌─────────┬───────┬───────┬─────────┬─────────────────────────────┐
│ Value   │ Good  │ Bad   │ Current │ t = (cur - Good)/(Bad-Good) │
├─────────┼───────┼───────┼─────────┼─────────────────────────────┤
│ Power   │ 100 % │ 0 %   │ 60 %    │ (60-100)/(0-100)   = 0.40   │
├─────────┼───────┼───────┼─────────┼─────────────────────────────┤
│ Heat    │ 20 °C │ 95 °C │ 84 °C   │ (84-20)/(95-20)    = 0.85   │
├─────────┼───────┼───────┼─────────┼─────────────────────────────┤
│ Traffic │ 40 %  │ 100 % │ 70 %    │ (70-40)/(100-40)   = 0.50   │
├─────────┼───────┼───────┼─────────┼─────────────────────────────┤
│ Disk    │ 0 %   │ 100 % │ 22 %    │ (22-0)/(100-0)     = 0.22   │
├─────────┼───────┼───────┼─────────┼─────────────────────────────┤
│ SLA     │ 100 % │ 0 %   │ 91 %    │ (91-100)/(0-100)   = 0.09   │
└─────────┴───────┴───────┴─────────┴─────────────────────────────┘


LERP (t -> value), check: putting t back in gives the Current value again
┌─────────┬───────┬───────┬──────┬───────────────────────────────┐
│ Value   │ Good  │ Bad   │  t   │ value = Good + (Bad-Good) * t │
├─────────┼───────┼───────┼──────┼───────────────────────────────┤
│ Power   │ 100 % │ 0 %   │ 0.40 │ 100 + (0-100)  * 0.40 = 60 %  │
├─────────┼───────┼───────┼──────┼───────────────────────────────┤
│ Heat    │ 20 °C │ 95 °C │ 0.85 │ 20  + (95-20)  * 0.85 = 84 °C │
├─────────┼───────┼───────┼──────┼───────────────────────────────┤
│ Traffic │ 40 %  │ 100 % │ 0.50 │ 40  + (100-40) * 0.50 = 70 %  │
├─────────┼───────┼───────┼──────┼───────────────────────────────┤
│ Disk    │ 0 %   │ 100 % │ 0.22 │ 0   + (100-0)  * 0.22 = 22 %  │
├─────────┼───────┼───────┼──────┼───────────────────────────────┤
│ SLA     │ 100 % │ 0 %   │ 0.09 │ 100 + (0-100)  * 0.09 = 91 %  │
└─────────┴───────┴───────┴──────┴───────────────────────────────┘

Good > Bad (Power, SLA) needs no special code: the two minus signs cancel.


t -> percentage: t * 100  (Heat: 0.85 * 100 = 85 % of the way to danger)

Careful: this is "how close to danger", NOT the value the player reads.
  - Display the real value on the row: Power 60 %, Heat 84 °C.
  - Use t (or t * 100) only for the colour and the bar fill.
  Showing "40 %" on the Power row would look like the battery is at 40 %, but it is at 60 %.

t -> colour (green -> orange -> red):
  t <  0.5 : Colors.Green.Lerp(Colors.Orange, t * 2)
  t >= 0.5 : Colors.Orange.Lerp(Colors.Red, (t - 0.5) * 2)



*/


using System;
using System.ComponentModel.DataAnnotations.Schema;
using Godot;

public sealed class dataStructure
{

    private int gPower = 100;
    private int gHeat = 20;
    private int gTraffic = 40;
    private int gDisk = 0;
    private int gSla = 100;

    private int bPower = 0;
    private int bHeat = 95;
    private int bTraffic = 100;
    private int bDisk = 100;
    private int bSla = 0;

    // 0 = good, 1 = bad. Use for the colour and bar fill, not for the displayed value.

    // this basically displays and uniforms all the value into t, then I can evaluate t properly
    public float HeatDanger(float heatValue)
    {
        return Mathf.Clamp(Mathf.InverseLerp(gHeat, bHeat, heatValue), 0f, 1f);
    }
    public float DiskDanger(float DiskValue)
    {
        return Mathf.Clamp(Mathf.InverseLerp(gDisk, bDisk, DiskValue), 0f, 1f);
    }
    public float PowerDanger(float Power)
    {
        return Mathf.Clamp(Mathf.InverseLerp(gPower, bPower, Power), 0f, 1f);
    }
    public float SLADanger(float SLA)
    {
        return Mathf.Clamp(Mathf.InverseLerp(gSla, bSla, SLA), 0f, 1f);
    }
    public float TrafficDanger(float Traffic)
    {
        return Mathf.Clamp(Mathf.InverseLerp(gTraffic, bTraffic, Traffic), 0f, 1f);
    }

}