using System.Numerics;
using CurrencyAlert.Classes;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;

namespace CurrencyAlert.Windows;

public class ConfigurationWindow : Window {
    private TrackedCurrency? selected;

    public ConfigurationWindow() : base("CurrencyAlert Configuration") {
        Size = new Vector2(700.0f, 450.0f);
        SizeCondition = ImGuiCond.FirstUseEver;
    }

    public override void Draw() {
        DrawCurrencyList();
        ImGui.SameLine();
        using (var child = ImRaii.Child("##rightSide", Vector2.Zero, true)) {
            if (child.Success) {
                DrawRightSide();
            }
        }
    }

    private void DrawCurrencyList() {
        using var child = ImRaii.Child("##currencyList", new Vector2(200.0f * ImGuiHelpers.GlobalScale, 0.0f), true);
        if (!child.Success) return;

        var buttonHeight = ImGui.GetFrameHeightWithSpacing();
        using (var listChild = ImRaii.Child("##list", new Vector2(0.0f, -buttonHeight))) {
            if (listChild.Success) {
                for (var i = 0; i < System.Config.Currencies.Count; i++) {
                    var currency = System.Config.Currencies[i];
                    using var id = ImRaii.PushId(i);

                    var label = currency switch {
                        { ItemId: 0, Type: CurrencyType.LimitedTomestone } => "Limited Tomestone (Currently Unavailable)",
                        _ => currency.Name,
                    };

                    if (ImGui.Selectable(label, selected == currency)) {
                        selected = currency;
                    }
                }
            }
        }

        if (ImGui.Button("Item##addItem")) OpenSearchWindow(CurrencyType.Item);
        ImGui.SameLine();
        if (ImGui.Button("HQ##addHq")) OpenSearchWindow(CurrencyType.HighQualityItem);
        ImGui.SameLine();
        if (ImGui.Button("Collectable##addCollectable")) OpenSearchWindow(CurrencyType.Collectable);
    }

    private void DrawRightSide() {
        using var tabBar = ImRaii.TabBar("##tabs");
        if (!tabBar.Success) return;

        using (var tab = ImRaii.TabItem("Currency")) {
            if (tab.Success) DrawSelectedCurrency();
        }

        using (var tab = ImRaii.TabItem("General")) {
            if (tab.Success) DrawGeneralSettings();
        }

        using (var tab = ImRaii.TabItem("Overlay Style")) {
            if (tab.Success) DrawOverlayStyle();
        }
    }

    private void DrawSelectedCurrency() {
        if (selected is null || !System.Config.Currencies.Contains(selected)) {
            selected = null;
            ImGui.TextDisabled("Select a currency from the list.");
            return;
        }

        var currency = selected;

        if (currency is { ItemId: 0, Type: CurrencyType.LimitedTomestone }) {
            ImGui.TextDisabled("Limited Tomestone (Currently Unavailable)");
            return;
        }

        DrawCurrentStatus(currency);
        ImGui.Separator();
        ImGuiHelpers.ScaledDummy(5.0f);

        var configChanged = false;

        configChanged |= ImGui.Checkbox("Enable", ref currency.Enabled);

        ImGuiHelpers.ScaledDummy(5.0f);

        configChanged |= Widgets.Checkbox("Chat Warning", ref currency.ChatWarning, "When amount is past threshold, print a message to chat when changing zones");
        configChanged |= Widgets.Checkbox("Invert", ref currency.Invert, "Warn when below the threshold instead of above");
        configChanged |= Widgets.Checkbox("Overlay", ref currency.ShowInOverlay, "Allows this currency to show in the overlay");
        configChanged |= Widgets.Checkbox("Overlay Show Name", ref currency.ShowItemName, "Show item name in the overlay");

        ImGuiHelpers.ScaledDummy(5.0f);

        ImGui.SetNextItemWidth(-1.0f);
        configChanged |= ImGui.InputTextWithHint("##WarningText", "Warning Text", ref currency.WarningText, 1024);

        ImGuiHelpers.ScaledDummy(5.0f);

        ImGui.SetNextItemWidth(100.0f * ImGuiHelpers.GlobalScale);
        configChanged |= ImGui.InputInt("Threshold", ref currency.Threshold, 0, 0);

        ImGuiHelpers.ScaledDummy(10.0f);

        using (ImRaii.Disabled(!(ImGui.GetIO().KeyShift && ImGui.GetIO().KeyCtrl && currency.CanRemove))) {
            if (ImGui.Button("Delete##delete", new Vector2(-1.0f, 0.0f))) {
                System.Config.Currencies.Remove(currency);
                selected = null;
                System.Config.Save();
                return;
            }
        }

        if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled)) {
            ImGui.SetTooltip(currency.CanRemove ? "Hold Shift + Control while clicking to delete this currency" : "Special currencies cannot be removed");
        }

        if (configChanged) {
            System.Config.Save();
        }
    }

    private static readonly Vector4 Red = new(1.0f, 0.25f, 0.25f, 1.0f);
    private static readonly Vector4 Orange = new(1.0f, 0.65f, 0.0f, 1.0f);
    private static readonly Vector4 OrangeRed = new(1.0f, 0.40f, 0.10f, 1.0f);
    private static readonly Vector4 White = Vector4.One;

    private static void DrawCurrentStatus(TrackedCurrency currency) {
        var currentCount = currency.CurrentCount;
        var threshold = currency.Threshold;

        var color = ((float) currentCount / threshold) switch {
            < 0.75f => currency.Invert ? Red : White,
            < 0.85f => currency.Invert ? Red : Orange,
            < 0.95f => currency.Invert ? Red : OrangeRed,
            > 0.95f and < 1.00f => Red,
            >= 1.00f and < 1.05f => currency.Invert ? OrangeRed : Red,
            >= 1.05f and < 1.15f => currency.Invert ? Orange : Red,
            >= 1.15f => currency.Invert ? White : Red,
            _ => White,
        };

        ImGui.TextUnformatted($"{currency.Name}");
        ImGui.SameLine();
        ImGui.TextColored(color, $"{currentCount:N0}");
        ImGui.SameLine();
        ImGui.TextUnformatted($"/ {threshold:N0}");

        ImGuiHelpers.ScaledDummy(5.0f);
    }

    private static void DrawGeneralSettings() {
        var configChanged = false;

        Widgets.Header("General Settings");
        configChanged |= ImGui.Checkbox("Enable Chat Warnings", ref System.Config.ChatWarning);

        Widgets.Header("Overlay Settings");
        configChanged |= ImGui.Checkbox("Enable Overlay", ref System.Config.OverlayEnabled);
        configChanged |= ImGui.Checkbox("Hide in Duties", ref System.Config.HideInDuties);
        configChanged |= Widgets.Checkbox("Disable Interaction", ref System.Config.DisableInteraction,
            "Makes the overlay click-through and undraggable.\nUncheck to move the overlay or click it to open this window.");

        if (configChanged) {
            System.Config.Save();
        }
    }

    private static void DrawOverlayStyle() {
        var config = System.Config;
        var configChanged = false;

        Widgets.Header("Layout");

        var position = config.OverlayPosition;
        if (ImGui.DragFloat2("Position", ref position, 0.75f, 0.0f, 5000.0f)) {
            config.OverlayPosition = position;
            configChanged = true;
        }

        var orientation = config.Orientation;
        if (Widgets.EnumCombo("Orientation", ref orientation)) {
            config.Orientation = orientation;
            configChanged = true;
        }

        configChanged |= ImGui.DragFloat("Item Spacing", ref config.ItemSpacing, 0.10f, 0.0f, 500.0f);
        configChanged |= ImGui.InputInt("Text Size", ref config.TextSize);

        Widgets.Header("Appearance");

        configChanged |= ImGui.ColorEdit4("Text Color", ref config.TextColor, ImGuiColorEditFlags.AlphaPreviewHalf);
        configChanged |= ImGui.Checkbox("Show Background", ref config.ShowBackground);
        configChanged |= ImGui.ColorEdit4("Background Color", ref config.BackgroundColor, ImGuiColorEditFlags.AlphaPreviewHalf);
        configChanged |= ImGui.Checkbox("Show Border", ref config.ShowBorder);
        configChanged |= ImGui.Checkbox("Show Icon", ref config.ShowIcon);
        configChanged |= ImGui.Checkbox("Show Text", ref config.ShowText);
        configChanged |= ImGui.Checkbox("Show Item Count", ref config.ShowItemCount);

        if (configChanged) {
            config.TextSize = int.Clamp(config.TextSize, 8, 96);
            config.Save();
        }
    }

    private static void OpenSearchWindow(CurrencyType type) {
        if (System.ItemSearchWindow is not null) {
            System.WindowSystem.RemoveWindow(System.ItemSearchWindow);
        }

        System.ItemSearchWindow = new ItemSearchWindow(type) {
            IsOpen = true,
        };
        System.WindowSystem.AddWindow(System.ItemSearchWindow);
    }

    public override void OnClose() {
        System.Config.Save();
    }
}
