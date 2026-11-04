# Idea: delivery dock and supply-chain attacks

Parked for M2/M3. Teaches supply-chain attacks (xz-utils style).

**Whats there:**
  - Loading bay with roll-up door and a little truck that backs in.
  - Inspection bench with a lamp, scanner and magnifying glass.
  - Quarantine cage, a fenced corner for anything suspicious.
  - Shelf of spate parts(drivers,cables,fans,RAM,power supplies, etc.).
  - Clipboard terminal listing expected deliveries,linked to the email on the main computer.
  - Security camera above the door.

**The core mechanic:**
  - Was it expected? Match it against the order list. A surprise package with no order is the first red flag.
  - Check the seal and box. It might be resealed tape, a dented box or a missing temper sticket.
  - Check the part itself. Look for the wrong serial number, a slightly off logo or label or  part number that doesnt match.
  - Decide. Install itm put it in quarantine or send it back.

**Incidents for the dock:**
  - Tempered drive. A replacement disk arrives with a broken seal.
  - Counterfeit network card. it works but fails after a few days. Fakes like this are a common problem in the real world.
  - Fake courier. A person shows up claiming to be a delivery driver but is not on the schedule. They may be trying to gain access to the building.
  - Critical part delayed. Your replacement part is delayed and you have to make a decision about whether to use a temporary workaround or wait for the part to arrive. This teaches why keeping spare parts matters.
  - Rain damage. The package is left out overnight. You decide whether its safe to use it.
  - Wrong part. The supplier sends the wrong part. Which teaches double-checking compatibility before you are in a hurry.

  **Upgrades you can unlock:**
    - Spare parts stock.
    - Trusted supplier contract. Fewer fake packages, but parts cost more. 
    - Serial number scanner. It flags mismatches automatically.
    - Security camera. It lets you see who is at the dock from the control room without walking down.
    - Delivery drone landing pad. FOR LATE GAME!

   **Day vs Night**
    - Day: normal deliveries arrive. You can restock shelves and inspect calmly.
    - Night: only emergency deliveries come, usually because you ordered a part during an incident. Thats when attackers strike, because you are tired and in a hurry.

**MAYBE Multiplayer**
One player insepcts at the dock and reads out serial numbers and seal details over the walkie-talkie. The other checks them against the order email and supplier records on the main computer. Neither can verify a package alone.
