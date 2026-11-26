> Research notes, not v1 rules. The v1 rules are in [game-v1.md](../game-v1.md); where this file differs, game-v1.md wins.

So what I'll be doing is working on the realism of the game. And also the game balancing so it'll involve a lot of in game testing.

**1.** research on how realistic data centers manage their infra structure and how I can implement it in the game without making it too hard or too easy

**2.** Think about the current game loop, and how to implement correctly each balancing change, which will require further testing.

**3.** Start implementing the logic and balancing for the in game shop and start researching on how we can upgrade stuff (is it on the computer or elsewhere?)


### Notes taking during research

  1. improved the fire logic, basically what causes fire are electrical issues not heat from the computers since computers have thermal throttling and major safety guidelines if the gpu or cpu gets too hot.
  2. Cloudflare stops DDOS attacks, and OVHcloud was a UPS/electrical fire.
  3. instead of sprinkler -> make a Clean-agent gas to stop fires.
  4. Increase the SLA count

### reality on how server works
- Temperature is measured at the server's air intake, not the room.
- Safe range is 18–27 °C for every standard server class.
- Servers protect themselves in stages: fans max out → CPU slows down (20–40% less performance) → automatic shutdown.
- With the AC off, a small room heats about 1 °C per minute and a big room about 2 °C per minute.
- Circuit boards don't burn until 300 °C or more, and cables until 250–450 °C. At 95 °C you'd get shutdowns and dead capacitors, not flames.

| Zone | Temp | Effect |
|---|---|---|
| Optimal | 18–27 °C | Normal |
| Warning | 27–32 °C | Warning email, loud fan audio, +20% power draw |
| Throttling | 32–44 °C | −30% traffic capacity, higher chance of hardware faults |
| Auto-Shutdown | ≥45 °C | Servers shut down safely with no fire. They stay off until the room is below 32 °C and you reboot. |

---
So in the game I'll need to implement so that instead of it going 20-100c, 
it goes from 18-55c, and the fire should be caused by an incident called "short circuit" or "electrical fire."


### dealing with the fire incident


first the player gets a warning that an electrical fire might happen and if the player ignores or doesn't patch the issue, then the issue persists and turns into an actual fire.

then after that, the player must use a fire extinguisher to put out the fire, and then continue doing other stuff.

#### in the shop

when a night ends the player can upgrade the fire and electrical issue detection for example

1. **Vesda Smoke Detection** which doubles the warning time before an electrical fault.
2. **Clean-Agent Gas Suppression** (premium): puts out basement fires automatically with no hardware damage


### dealing with power consumption and organisation

> Since solar panels only deal with power that work during the day that charges a battery, you'll be able to either upgrade the solar panels or buy a diesel generator that can charge the batteries over night

**risks assesment:**

> however once the player buys the diesel generator then he has to maintain it too, and since his server will have more servers/clients. then power will drain faster hence making the game harder.


### Game
- Keep Power as a 0–100% battery gauge.
- **20:00–06:00:** the battery only drains (based on stage, traffic and AC).
- **06:00–08:00:** solar recharges it. Dirty panels cut recharge by 30–60%, which can cause a blackout at the end of the shift or start the next night half-empty.
- **15% battery:** warning that unlocks **Eco / Load-Shed Mode** (traffic capped at 70%, 30% less drain).
- **0% battery:** the UPS takes over for **15 seconds**.
  - **Graceful Shutdown** clicked in time: clean 10-second reboot when power returns.
  - UPS runs out: **Hard Crash** with a 30-second disk-recovery penalty.

### the incidents
| # | Incident | Real Event | Warning Sign | Upgrade Change |
|---|---|---|---|---|
| 1 | Expired TLS cert | MS Teams 2020 ✅ | `TLS cert expires in 14h — certbot.timer failed` | Auto-renewal blocks 85% |
| 2 | DDoS | **Change to Dyn 2016 / GitHub 2018** | Ransom email or traffic spike | CDN Shield blocks 90% |
| 3 | Vulnerable dependency | Log4Shell 2021 ✅ | Critical CVE advisory or WAF alert | Scanner doubles warning time |
| 4 | Bad update | CrowdStrike 2024 ✅ | Rollout alert | Staged rollouts limit it to 1 node |
| 5 | Dirty panels | Dust or snow ✅ | Weather alert | Rename upgrade to Wiper Robots |
| 6 | Cooling failure | **Change to Google/Oracle London 2022** | Temperature creep, loud fans | Second AC prevents it |
| 7 | Failing drive | Backblaze stats ✅ | `SMART 197 rising on /dev/sdb (Bay 3)` | RAID stops SLA drain |
| 8 | Phishing | Verizon DBIR ✅ | Suspicious email in inbox | FIDO2 keys block 95%+ |
    


