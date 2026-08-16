# CurrencyAlert

[中文](README.md)

A FFXIV Dalamud plugin that warns you when your currencies reach configurable thresholds, preventing precious resources from going to waste.

> A "currency" can be any item in the game — the game treats currencies like gil as items internally, so this plugin can track any item.

The original author [MidoriKami](https://github.com/MidoriKami/CurrencyAlert) no longer maintains the plugin; this repository is a community-maintained fork updated for the current game version (7.5 / Dalamud API 15).

## Features

**Chat warnings**: prints a message to chat when you change zones (5-minute internal cooldown).

**Overlay**: shows currencies currently triggering a warning, with icon, name and current count. Draggable, optional click-through, horizontal/vertical layout, configurable text size, colors and background, optional auto-hide in duties.

## Installation

1. Download `latest.zip` from [Releases](https://github.com/aethel-tail/CurrencyAlert/releases)
2. Extract into Dalamud's `devPlugins` directory (`%APPDATA%\XIVLauncher\devPlugins\CurrencyAlert\`)
3. `/xlsettings` → Experimental → enable developer plugin support
4. Enable CurrencyAlert in `/xlplugins`

## Usage

| Command | Description |
|---------|-------------|
| `/currencyalert` / `/calert` | Open the configuration window |

Use the buttons at the bottom of the currency list to add tracked items: normal items, high-quality items, or collectables. Each currency has its own threshold, chat warning toggle, overlay toggle, and invert option (warn when *below* the threshold instead of above).

Clicking the overlay (unless interaction is disabled) also opens the configuration window.

## Build

```bash
dotnet build                 # Debug
dotnet build -c Release      # Release
```

Requires Dalamud SDK 15.0.0, targets .NET 10, x64. A local Dalamud installation is required (via the `DALAMUD_HOME` environment variable or the default XIVLauncher install path).

## License

MIT (inherited from the original repository)

## Acknowledgments

- [MidoriKami](https://github.com/MidoriKami) and Lharz: original work
- This fork is maintained by aethel-tail, with the API 15 adaptation and rewrite assisted by Kimi K3
