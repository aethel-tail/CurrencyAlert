using CurrencyAlert.Classes;
using CurrencyAlert.Windows;
using Dalamud.Interface.Windowing;

namespace CurrencyAlert;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public static class System {
    public static Configuration Config { get; set; }
    public static WindowSystem WindowSystem { get; set; }
    public static ConfigurationWindow ConfigurationWindow { get; set; }
    public static OverlayWindow OverlayWindow { get; set; }
    public static ItemSearchWindow? ItemSearchWindow { get; set; }
}
