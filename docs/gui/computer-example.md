# Example: the desk computer

Part of the [GUI guide](../agents_gui_guide.md).

`dusk-till-dawn/main/computer_ui/` applies the style as a diegetic OS desktop (NixOS + GNOME wallpaper, top bar with live clock, app icons with labels). The desktop theme belongs only to the computer. Other components take the style from it, not the desktop.

| File | Role |
|---|---|
| `computer_menu.tscn` | Backdrop, framed screen, desktop, three app icons |
| `ComputerMenu.cs` | Open animation, clock, hover, `MailPressed` / `MonitoringPressed` / `HelpPressed` signals |
| `../ui/BlockReveal.cs` | Block reveal (shared with the Mail and Monitoring windows) |
| `../DetectPlayerNearComputer.cs` | Trigger: shows "press E" when the player is in range, opens on E, hides on exit |
| `../mail_ui/` (`mail_app.tscn`, `MailApp.cs`, `MailController.cs`) | Mail window inside `Map/Screen`; the controller connects it to `GameState.Inbox` |
| `../monitor_ui/` (`monitor_app.tscn`, `MonitorApp.cs`, `MonitoringController.cs`) | Monitoring window inside `Map/Screen`; the controller feeds it the 5 values |
