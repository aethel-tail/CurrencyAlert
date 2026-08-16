using System.Linq;
using System.Numerics;
using CurrencyAlert.Classes;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;

namespace CurrencyAlert.Windows;

public class OverlayWindow : Window {
    private bool wasDragging;

    public OverlayWindow() : base("CurrencyAlert Overlay") {
        Flags = ImGuiWindowFlags.NoTitleBar
              | ImGuiWindowFlags.NoResize
              | ImGuiWindowFlags.AlwaysAutoResize
              | ImGuiWindowFlags.NoScrollbar
              | ImGuiWindowFlags.NoCollapse
              | ImGuiWindowFlags.NoSavedSettings
              | ImGuiWindowFlags.NoFocusOnAppearing
              | ImGuiWindowFlags.NoBringToFrontOnFocus
              | ImGuiWindowFlags.NoNav;

        RespectCloseHotkey = false;
        IsOpen = false;
    }

    public override void PreDraw() {
        var config = System.Config;

        if (config.DisableInteraction) Flags |= ImGuiWindowFlags.NoInputs;
        else Flags &= ~ImGuiWindowFlags.NoInputs;

        ImGui.SetNextWindowPos(config.OverlayPosition, ImGuiCond.Always);
        ImGui.PushStyleColor(ImGuiCol.WindowBg, config.ShowBackground ? config.BackgroundColor : Vector4.Zero);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, config.ShowBorder ? 1.0f : 0.0f);
    }

    public override void PostDraw() {
        ImGui.PopStyleColor();
        ImGui.PopStyleVar();
    }

    public override void Draw() {
        var config = System.Config;
        var fontScale = config.TextSize / ImGui.GetFontSize();
        ImGui.SetWindowFontScale(fontScale);

        var warnings = config.Currencies
            .Where(currency => currency is { HasWarning: true, Enabled: true, ShowInOverlay: true })
            .ToList();

        if (warnings.Count is 0) {
            // Keep the overlay findable/draggable while interactive
            if (!config.DisableInteraction) {
                ImGui.TextDisabled("CurrencyAlert");
            }
        }
        else {
            for (var i = 0; i < warnings.Count; i++) {
                if (i > 0) {
                    if (config.Orientation is OverlayOrientation.Horizontal) {
                        ImGui.SameLine(0.0f, config.ItemSpacing * fontScale);
                    }
                    else {
                        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + config.ItemSpacing * fontScale);
                    }
                }

                DrawWarning(warnings[i], config);
            }
        }

        HandleDragAndClick(config);
    }

    private static void DrawWarning(TrackedCurrency currency, Configuration config) {
        ImGui.BeginGroup();

        if (config.ShowIcon) {
            var iconSize = config.TextSize * 4.0f / 3.0f;
            ImGui.Image(currency.Icon.Handle, new Vector2(iconSize));

            if (config.ShowText || config.ShowItemCount) {
                ImGui.SameLine(0.0f, config.TextSize / 3.0f);
                // Roughly center the text vertically against the icon
                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + (iconSize - ImGui.GetTextLineHeight()) / 2.0f);
            }
        }

        if (config.ShowText) {
            var text = currency.ShowItemName ? $"{currency.Name} {currency.WarningText}" : currency.WarningText;
            if (config.ShowItemCount) {
                text = $"{text} ({currency.CurrentCount:N0})";
            }
            ImGui.TextColored(config.TextColor, text);
        }
        else if (config.ShowItemCount) {
            ImGui.TextColored(config.TextColor, $"{currency.CurrentCount:N0}");
        }

        ImGui.EndGroup();
    }

    private void HandleDragAndClick(Configuration config) {
        if (config.DisableInteraction) return;

        var hovered = ImGui.IsWindowHovered();

        if (hovered && ImGui.IsMouseDragging(ImGuiMouseButton.Left)) {
            config.OverlayPosition += ImGui.GetIO().MouseDelta;
            wasDragging = true;
        }

        if (ImGui.IsMouseReleased(ImGuiMouseButton.Left)) {
            if (wasDragging) {
                config.Save();
            }
            else if (hovered) {
                System.ConfigurationWindow.Toggle();
            }
            wasDragging = false;
        }
    }
}
