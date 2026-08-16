using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text.Json;
using Dalamud.Configuration;

namespace CurrencyAlert.Classes;

public enum OverlayOrientation {
    Vertical,
    Horizontal,
}

public class Configuration : IPluginConfiguration {
    public bool ChatWarning = false;

    public List<TrackedCurrency> Currencies = [];
    public bool HideInDuties = false;
    public bool DisableInteraction = false;
    public bool OverlayEnabled = true;

    // Overlay style settings
    public Vector2 OverlayPosition = new(960.0f, 512.0f);
    public OverlayOrientation Orientation = OverlayOrientation.Vertical;
    public float ItemSpacing = 10.0f;
    public int TextSize = 24;
    public Vector4 TextColor = Vector4.One;
    public bool ShowBackground = false;
    public Vector4 BackgroundColor = new(0.0f, 0.0f, 0.0f, 0.30f);
    public bool ShowBorder = false;
    public bool ShowIcon = true;
    public bool ShowText = true;
    public bool ShowItemCount = true;

    public int Version { get; set; } = 8;

    private static readonly JsonSerializerOptions SerializerOptions = new() {
        IncludeFields = true,
        WriteIndented = true,
    };

    private static string ConfigPath => Path.Combine(Service.PluginInterface.ConfigDirectory.FullName, "CurrencyAlert.config.json");

    public static Configuration Load() {
        try {
            var json = File.ReadAllText(ConfigPath);
            return JsonSerializer.Deserialize<Configuration>(json, SerializerOptions) ?? new Configuration();
        }
        catch (IOException) {
            return new Configuration();
        }
        catch (JsonException ex) {
            Service.Log.Warning(ex, "Failed to parse config file, starting with defaults.");
            return new Configuration();
        }
    }

    public void Save() {
        var json = JsonSerializer.Serialize(this, SerializerOptions);
        File.WriteAllText(ConfigPath, json);
    }
}
