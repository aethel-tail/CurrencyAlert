using System.Collections.Generic;
using System.Linq;
using CurrencyAlert.Classes;
using CurrencyAlert.Windows;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace CurrencyAlert;

public sealed class CurrencyAlertPlugin : IDalamudPlugin {
    public CurrencyAlertPlugin(IDalamudPluginInterface pluginInterface) {
        pluginInterface.Create<Service>();

        System.Config = Configuration.Load();

        if (System.Config.Currencies is null or { Count: 0 }) {
            Service.Log.Verbose("Generating Initial Currency List.");

            System.Config.Currencies = GenerateInitialList();
            System.Config.Save();
        }

        System.WindowSystem = new WindowSystem("CurrencyAlert");
        System.ConfigurationWindow = new ConfigurationWindow();
        System.OverlayWindow = new OverlayWindow();
        System.WindowSystem.AddWindow(System.ConfigurationWindow);
        System.WindowSystem.AddWindow(System.OverlayWindow);

        Service.PluginInterface.UiBuilder.Draw += System.WindowSystem.Draw;
        Service.PluginInterface.UiBuilder.OpenConfigUi += System.ConfigurationWindow.Toggle;

        Service.CommandManager.AddHandler("/currencyalert", new CommandInfo(OnCommand) {
            HelpMessage = "Open CurrencyAlert configuration.",
        });
        Service.CommandManager.AddHandler("/calert", new CommandInfo(OnCommand) {
            HelpMessage = "Open CurrencyAlert configuration.",
        });

        Service.ClientState.TerritoryChanged += OnZoneChange;
        Service.Framework.Update += OnFrameworkUpdate;
    }

    public void Dispose() {
        Service.ClientState.TerritoryChanged -= OnZoneChange;
        Service.Framework.Update -= OnFrameworkUpdate;

        Service.PluginInterface.UiBuilder.Draw -= System.WindowSystem.Draw;
        Service.PluginInterface.UiBuilder.OpenConfigUi -= System.ConfigurationWindow.Toggle;

        Service.CommandManager.RemoveHandler("/currencyalert");
        Service.CommandManager.RemoveHandler("/calert");
    }

    private static void OnCommand(string command, string args)
        => System.ConfigurationWindow.Toggle();

    private void OnFrameworkUpdate(IFramework framework) {
        if (!Service.ClientState.IsLoggedIn) {
            System.OverlayWindow.IsOpen = false;
            return;
        }

        System.OverlayWindow.IsOpen = System.Config.OverlayEnabled
            && !Service.GameGui.GameUiHidden
            && !(Service.Condition[ConditionFlag.BoundByDuty] && System.Config.HideInDuties);
    }

    private void OnZoneChange(uint e) {
        if (System.Config is { ChatWarning: false }) return;

        foreach (var currency in System.Config.Currencies.Where(currency => currency is { HasWarning: true, ChatWarning: true, Enabled: true })) {
            Service.ChatGui.Print($"{currency.Name} is {(currency.Invert ? "below" : "above")} threshold.", "CurrencyAlert", 43);
        }
    }

    private static List<TrackedCurrency> GenerateInitialList() => [
        new() { Type = CurrencyType.Item, ItemId = 20, Threshold = 75000, Enabled = true, }, // StormSeal
        new() { Type = CurrencyType.Item, ItemId = 21, Threshold = 75000, Enabled = true, }, // SerpentSeal
        new() { Type = CurrencyType.Item, ItemId = 22, Threshold = 75000, Enabled = true, }, // FlameSeal

        new() { Type = CurrencyType.Item, ItemId = 25, Threshold = 18000, Enabled = true, }, // WolfMarks
        new() { Type = CurrencyType.Item, ItemId = 36656, Threshold = 18000, Enabled = true, }, // TrophyCrystals

        new() { Type = CurrencyType.Item, ItemId = 27, Threshold = 3500, Enabled = true, }, // AlliedSeals
        new() { Type = CurrencyType.Item, ItemId = 10307, Threshold = 3500, Enabled = true, }, // CenturioSeals
        new() { Type = CurrencyType.Item, ItemId = 26533, Threshold = 3500, Enabled = true, }, // SackOfNuts

        new() { Type = CurrencyType.Item, ItemId = 26807, Threshold = 800, Enabled = true, }, // BicolorGemstones

        new() { Type = CurrencyType.Item, ItemId = 28, Threshold = 1400, Enabled = true, }, // Poetics
        new() { Type = CurrencyType.NonLimitedTomestone, Threshold = 1400, Enabled = true, }, // NonLimitedTomestone
        new() { Type = CurrencyType.LimitedTomestone, Threshold = 1400, Enabled = true, }, // LimitedTomestone

        new() { Type = CurrencyType.Item, ItemId = 28063, Threshold = 7500, Enabled = true, }, // Skybuilders scripts
    ];
}
