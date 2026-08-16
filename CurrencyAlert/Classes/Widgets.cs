using System;
using System.Collections.Generic;
using Dalamud.Bindings.ImGui;

namespace CurrencyAlert.Classes;

public static class Widgets {
    public static bool Checkbox(string label, ref bool value, string? helpText = null) {
        var changed = ImGui.Checkbox(label, ref value);
        if (helpText is not null && ImGui.IsItemHovered()) {
            ImGui.SetTooltip(helpText);
        }
        return changed;
    }

    public static void Header(string label) {
        ImGui.TextDisabled(label);
        ImGui.Separator();
    }

    public static bool EnumCombo<T>(string label, ref T value) where T : struct, Enum {
        var changed = false;
        if (ImGui.BeginCombo(label, value.ToString())) {
            foreach (var option in Enum.GetValues<T>()) {
                if (ImGui.Selectable(option.ToString(), EqualityComparer<T>.Default.Equals(option, value))) {
                    value = option;
                    changed = true;
                }
            }
            ImGui.EndCombo();
        }
        return changed;
    }
}
