using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using CurrencyAlert.Classes;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Lumina.Excel.Sheets;

namespace CurrencyAlert.Windows;

public class ItemSearchWindow : Window {
    private const int MaxResults = 100;

    private readonly CurrencyType type;
    private readonly HashSet<uint> selected = [];
    private string searchText = string.Empty;
    private string? lastSearch;
    private List<Item> matches = [];

    public ItemSearchWindow(CurrencyType type) : base($"Add Tracked Currency - {type}") {
        this.type = type;
        Size = new Vector2(400.0f, 500.0f) * ImGuiHelpers.GlobalScale;
        SizeCondition = ImGuiCond.FirstUseEver;
    }

    public override void Draw() {
        ImGui.SetNextItemWidth(-1.0f);
        ImGui.InputTextWithHint("##search", "Search items...", ref searchText, 256);

        if (searchText != lastSearch) {
            lastSearch = searchText;
            matches = ComputeMatches();
        }

        using (var child = ImRaii.Child("##results", new Vector2(0.0f, -ImGui.GetFrameHeightWithSpacing()))) {
            if (child.Success) {
                foreach (var item in matches) {
                    var isSelected = selected.Contains(item.RowId);
                    if (ImGui.Selectable($"{item.Name.ExtractText()}##{item.RowId}", isSelected)) {
                        if (isSelected) selected.Remove(item.RowId);
                        else selected.Add(item.RowId);
                    }
                }

                if (matches.Count >= MaxResults) {
                    ImGui.TextDisabled($"(showing first {MaxResults} results, refine your search)");
                }
            }
        }

        using (ImRaii.Disabled(selected.Count is 0)) {
            if (ImGui.Button($"Add {selected.Count} item(s)##add", new Vector2(-1.0f, 0.0f))) {
                AddSelected();
                IsOpen = false;
            }
        }
    }

    private List<Item> ComputeMatches() {
        var results = new List<Item>(MaxResults);
        foreach (var item in Service.DataManager.GetExcelSheet<Item>()) {
            if (item.RowId is 0) continue;

            var name = item.Name.ExtractText();
            if (name.Length is 0) continue;

            if (searchText.Length > 0 && !name.Contains(searchText, StringComparison.OrdinalIgnoreCase)) continue;

            results.Add(item);
            if (results.Count >= MaxResults) break;
        }
        return results;
    }

    private void AddSelected() {
        foreach (var itemId in selected.Where(itemId => System.Config.Currencies.All(currency => currency.ItemId != itemId))) {
            System.Config.Currencies.Add(new TrackedCurrency {
                Enabled = true,
                Threshold = 1000,
                Type = type,
                ItemId = itemId,
            });
        }
        System.Config.Save();
    }
}
