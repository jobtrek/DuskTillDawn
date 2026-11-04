# Idea: incident catalogue

Full brainstorm list, by floor. The 8 chosen for v1 are in [game-v1.md](../game-v1.md#8-the-8-incidents-for-v1). Everything else is a candidate for later.

Marked **(v1)** = already in v1.

## Roof
- Dirty / snow-covered solar panels **(v1)**
- Inverter failure
- Storm or hail damage
- Lightning strike
- Battery overheating
- Radio antenna misaligned (backup link down)

## Control room
- DDoS attack **(v1)**
- Vulnerable dependency, like Log4Shell **(v1)**
- Supply chain backdoor, like xz-utils (needs the [delivery dock](delivery-dock.md))
- Bad update crashes everything, like CrowdStrike **(v1)**
- Expired TLS certificate **(v1)**
- Brute force logins
- SQL injection
- Leaked API key
- Phishing email **(v1)**
- Disk full from logs

## Basement
- Server fire, like OVHcloud **(v1, as cooling failure → fire)**
- Cooling failure **(v1)**
- Failing hard drive **(v1)**
- Flooding
- Unplugged / chewed cable
- Ransomware

## Cascades

- Dirty panels → less power → slow servers → SLA drops **(v1)**
- Cooling fails → overheating → fire **(v1)**
- DDoS → more heat and power draw → battery drains **(v1)**

## Types of attack

Attacks can differ: code, infrastructure, or natural disaster (a server catching fire, solar panels not working).

## Learning angle

Incidents reflect real events, simplified (a recent PHP incident, a DDoS attempt on Cloudflare). The player re-enacts the fix.

## Other incident sources

- Dock: tampered drive, counterfeit network card, fake courier, delayed part, rain damage, wrong part → [delivery-dock.md](delivery-dock.md)
- Generator: won't start, empty tank, overload, long outage, exhaust blocked, stolen fuel → [generator-yard.md](generator-yard.md)
- Weather and animals → [weather-and-nature.md](weather-and-nature.md)
