# Monitor Service Math

## Values to store

The monitor service tracks 5 values.

| Value   | Range                         | Orange (close to limit) | Red (past limit)                |
| ------- | ----------------------------- | ----------------------- | ------------------------------- |
| Power   | 0 to 100 %                    | Not decided yet         | 0 % = servers offline           |
| Heat    | 20 to 100 °C                  | Not decided yet         | 80 °C = fire risk, 95 °C = fire |
| Traffic | 0 to 100 % (40 % is normal)   | Not decided yet         | 100 % = requests dropped        |
| Disk    | 0 to 100 %                    | Not decided yet         | 100 % = crash                   |
| SLA     | 0 to 100 %                    | Not decided yet         | 0 % = game lost                 |

## Storage and queries

- Store the values in a `Dictionary` with the type as key and the value as value.
- Write public query methods that fetch the values.
- Update the state in `GameState`.

## Linear interpolation

To choose between green, orange and red, use linear interpolation.
It measures where a value sits between 2 points on a straight line.



```
t = (value - Good) / (Bad - Good)
```

- `t` is how far the value has moved from Good toward Bad.
- `t = 0` at Good, `t = 1` at Bad.
- Clamp `t` to `[0, 1]`.

Each value has its own Good and Bad:

| Value   | Good  | Bad   |
| ------- | ----- | ----- |
| Power   | 100 % | 0 %   |
| Heat    | 20 °C | 95 °C |
| Traffic | 40 %  | 100 % |
| Disk    | 0 %   | 100 % |
| SLA     | 100 % | 0 %   |

## Clamping

Clamping limits a value to a range between a minimum and a maximum.

## Lerp and inverse lerp

They are the same line, read in opposite directions.

| Function     | Direction  | Formula                             | Godot                               |
| ------------ | ---------- | ----------------------------------- | ----------------------------------- |
| Inverse lerp | value to t | `t = (value - Good) / (Bad - Good)` | `Mathf.InverseLerp(Good, Bad, value)` |
| Lerp         | t to value | `value = Good + (Bad - Good) * t`   | `Mathf.Lerp(Good, Bad, t)`          |

Example with Heat:

```
Good = 20                              value = 84      Bad = 95
|----------------------------------------|---------------|
t = 0                                  t = 0.85        t = 1
```

### Inverse lerp (value to t)

| Value   | Good  | Bad   | Current | t = (Current - Good) / (Bad - Good) |
| ------- | ----- | ----- | ------- | ----------------------------------- |
| Power   | 100 % | 0 %   | 60 %    | (60 - 100) / (0 - 100) = 0.40       |
| Heat    | 20 °C | 95 °C | 84 °C   | (84 - 20) / (95 - 20) = 0.85        |
| Traffic | 40 %  | 100 % | 70 %    | (70 - 40) / (100 - 40) = 0.50       |
| Disk    | 0 %   | 100 % | 22 %    | (22 - 0) / (100 - 0) = 0.22         |
| SLA     | 100 % | 0 %   | 91 %    | (91 - 100) / (0 - 100) = 0.09       |

### Lerp (t to value)

Putting `t` back in gives the current value again.

| Value   | Good  | Bad   | t    | value = Good + (Bad - Good) * t  |
| ------- | ----- | ----- | ---- | -------------------------------- |
| Power   | 100 % | 0 %   | 0.40 | 100 + (0 - 100) * 0.40 = 60 %    |
| Heat    | 20 °C | 95 °C | 0.85 | 20 + (95 - 20) * 0.85 = 84 °C    |
| Traffic | 40 %  | 100 % | 0.50 | 40 + (100 - 40) * 0.50 = 70 %    |
| Disk    | 0 %   | 100 % | 0.22 | 0 + (100 - 0) * 0.22 = 22 %      |
| SLA     | 100 % | 0 %   | 0.09 | 100 + (0 - 100) * 0.09 = 91 %    |

When Good is bigger than Bad (Power, SLA), no special code is needed. The two minus signs cancel.

## t to percentage

```
percentage = t * 100
```

Example: Heat `0.85 * 100 = 85 %` of the way to danger.

This is how close the value is to danger. It is not the value the player reads.

- Show the real value on the row: Power 60 %, Heat 84 °C.
- Use `t` (or `t * 100`) only for the colour and the bar fill.
- Showing 40 % on the Power row would look like the battery is at 40 %. It is at 60 %.

## t to colour

Green to orange to red:

```csharp
if (t < 0.5f)
{
    color = Colors.Green.Lerp(Colors.Orange, t * 2);
}
else
{
    color = Colors.Orange.Lerp(Colors.Red, (t - 0.5f) * 2);
}
```
