# CurrencyAlert

[English](README.en.md)

FFXIV Dalamud 插件。当游戏内货币数量达到你设定的阈值时给出提醒，防止资源溢出浪费。

> “货币”可以是任何物品——游戏内部把金币这类货币也当作物品处理，因此本插件可以追踪任意物品。

原作者 [MidoriKami](https://github.com/MidoriKami/CurrencyAlert) 已停止维护，本仓库为适配当前游戏版本（7.5 / Dalamud API15）的社区维护分支。

## 功能

**聊天提醒**：切换地图时在聊天框提示已超阈值的货币（内置 5 分钟冷却）。

**悬浮 Overlay**：在屏幕上以图标 + 文字实时显示触发警告的货币及当前数量。位置可拖拽，可点击穿透，支持横向/竖向排列、字号、颜色、背景等样式设置，可设置在副本中自动隐藏。

## 安装

1. 在 [Releases](https://github.com/aethel-tail/CurrencyAlert/releases) 下载 `latest.zip`
2. 解压到 Dalamud 的 `devPlugins` 目录（`%APPDATA%\XIVLauncher\devPlugins\CurrencyAlert\`）
3. `/xlsettings` → 实验性 → 启用开发者插件支持
4. `/xlplugins` 中启用 CurrencyAlert

## 使用方法

| 命令 | 说明 |
|------|------|
| `/currencyalert` / `/calert` | 打开配置窗口 |

在配置窗口左侧列表底部可添加三种追踪项：普通物品（Item）、HQ 物品、收集品（Collectable）。每个货币可独立设置阈值、聊天提醒、是否进 Overlay、反向提醒（低于阈值时警告，如“该买食物了”）。

点击 Overlay（未开启“禁止交互”时）也可快速打开配置窗口。

## 构建

```bash
dotnet build                 # Debug
dotnet build -c Release      # Release
```

依赖：Dalamud SDK 15.0.0，目标 .NET 10，x64。需要本机存在 Dalamud（通过 `DALAMUD_HOME` 环境变量指定，或默认的 XIVLauncher 安装路径）。

## 许可

MIT（沿用原仓库许可）

## 致谢

- [MidoriKami](https://github.com/MidoriKami) 与 Lharz：原作
- 本分支由 aethel-tail 维护，API15 适配与重写工作由 Kimi K3 协助完成
