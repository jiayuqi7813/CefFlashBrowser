# CrossPlatformCefFlashBrowser

该解决方案是对原 Windows 专用 CefFlashBrowser 的跨平台重写初始版本，基于 .NET MAUI 构建，目标支持 Windows、macOS，并为 Linux 预留扩展空间。

## 项目结构

- `CrossPlatformCefFlashBrowser.sln`：解决方案文件。
- `src/CrossPlatformCefFlashBrowser/`：主 .NET MAUI 应用，包含 UI、服务与平台特定代码。
- `src/CrossPlatformCefFlashBrowser.Core/`：共享业务逻辑、MVVM 等核心模块（当前包含依赖注入扩展）。
- `src/CrossPlatformCefFlashBrowser.Sol/`：纯 C# 实现的 SOL/AMF0 解析与写入逻辑。
- `src/CrossPlatformCefFlashBrowser.Tests/`：预留的单元测试项目目录。
- `docs/`：项目文档与设计说明。
- `assets/`：静态资源占位目录。

## 关键特性

- 使用 `CommunityToolkit.Mvvm` 实现 MVVM 绑定。
- 提供 `ISettingsService`、`IFavoritesService`、`INavigationService` 等基础服务接口与默认实现。
- 集成可扩展的 `IFlashPlayerService` 和 `IWebViewBridge`，便于后续嵌入 Ruffle 等 Flash 模拟器。
- `SolFile` 使用 Span API 与自定义二进制 reader/writer，兼容 AMF0 常见数据类型。

## 快速开始

1. 安装 [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download) 及对应平台工作负载（`dotnet workload install maui`）。
2. 进入仓库根目录运行：
   ```bash
   dotnet restore CrossPlatformCefFlashBrowser/CrossPlatformCefFlashBrowser.sln
   ```
3. 使用 Visual Studio 2022 17.8+ 或 `dotnet build` 编译 MAUI 项目。
4. 后续可根据 `docs/rewrite-plan.md` 中的指引继续迁移功能。

> 当前代码尚未集成真实的 Flash 播放实现与平台特定的 WebView 扩展，仍需在后续迭代中完成。
