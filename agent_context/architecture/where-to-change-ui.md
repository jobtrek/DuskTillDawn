# Where do I change X? (UI)

Part of [where-to-change.md](where-to-change.md). Paths from the repo root. `dtd/` = `dusk-till-dawn/`.

| To change… | Go to |
|---|---|
| Mail window (view) / its logic | `dtd/main/mail_ui/MailApp.cs` / `MailController.cs` |
| Monitoring window (Metrics and Servers tabs) / its logic | `dtd/main/monitor_ui/MonitorApp.cs` / `MonitoringController.cs` |
| Admin window (traffic filter, Power: faulty circuit, Eco Mode, Graceful shutdown, busy countdown after a wrong action) / what each does | `dtd/main/admin_ui/AdminApp.cs` / `AdminController.cs` |
| Desk computer menu, app icons, signals; block reveal | `dtd/main/computer_ui/ComputerMenu.cs`, `computer_menu.tscn`; `BlockReveal.cs` |
| Shared UI motion (hover, App window open, lift-the-map open, Esc close, HUD pop and slide-in, loss/gain tints) | `dtd/main/UiMotion.cs` |
| "Wrong action: busy N s" wording (admin console, rack, roof panel, extinguisher prompts) | `dtd/main/BusyText.cs` |
| When the computer opens (player nearby) | `dtd/main/DetectPlayerNearComputer.cs` |
| Dashboard (HUD panel: points, failures, running challenges) | `dtd/main/dashboard_ui/Dashboard.cs`, `dashboard.tscn` |
| HUD: clock, SLA bar, points, banner / night report at 08:00 | `dtd/main/hud_ui/Hud.cs`, `SlaBar.cs`, `hud.tscn` / `dtd/main/night_report_ui/`, `game-core/NightReport.cs`, when: `Main.cs` |
| UI pixel textures | `scripts/gen_computer_ui_textures.py` → `dtd/main/computer_ui/textures/` (HUD: `dtd/main/hud_ui/textures/`) |

How to build or review UI: [../gui/README.md](../gui/README.md). Not found here? Add the row after you find it.
