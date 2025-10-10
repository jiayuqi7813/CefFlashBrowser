# Cross-Platform CefFlashBrowser 重写规划

本文档概述了将原有 WPF 版 CefFlashBrowser 迁移到 .NET MAUI 跨平台架构的整体策略、阶段性目标与当前仓库中已经初始化的基础代码。该内容将作为团队协作与后续迭代的基线。 

## 目标概览

- ✅ 构建可在 Windows 与 macOS 运行的单一 .NET MAUI 解决方案，并为 Linux 保留扩展接口。
- ✅ 提供与原项目一致的数据模型与服务接口，以便逐步迁移业务逻辑。
- ✅ 使用纯 C# 实现 SOL 文件解析与写入逻辑，替代原生 C++ 组件。
- ✅ 为 Flash 播放提供 Ruffle 等模拟器的接入预留接口。
- ✅ 构建设置、收藏夹等基础服务，确保与原有数据结构兼容。

## 解决方案结构

```
CrossPlatformCefFlashBrowser/
├── CrossPlatformCefFlashBrowser.sln
├── src/
│   ├── CrossPlatformCefFlashBrowser/          # .NET MAUI 主项目
│   ├── CrossPlatformCefFlashBrowser.Core/     # 业务与 MVVM 相关逻辑
│   ├── CrossPlatformCefFlashBrowser.Sol/      # SOL 格式解析/写入
│   └── CrossPlatformCefFlashBrowser.Tests/    # 测试项目（预留）
├── assets/                                    # 静态资源（预留）
└── docs/                                      # 文档
```

### 平台目录说明

- `Platforms/Windows`：包含 WinUI 启动代码。
- `Platforms/MacCatalyst`：包含 Mac Catalyst 的入口点。
- `Platforms/Linux`：当前抛出未实现异常，后续将引入 Gtk/Skia 驱动或 Uno/Avalonia 外壳。

## 当前进展摘要

- ✅ 初始化了 .NET MAUI 单项目结构以及基础依赖（CommunityToolkit.MVVM 等）。
- ✅ 创建主页面 `MainPage` 与 `MainViewModel`，提供基本的浏览器工具栏与 WebView 绑定。
- ✅ 初始化设置、收藏夹、导航、文件访问等服务接口，并提供默认实现，支持依赖注入。
- ✅ 建立 Flash 播放桥接接口 `IFlashPlayerService` 与 `IWebViewBridge`，预留与 Ruffle 集成的扩展点。
- ✅ 使用纯 C# 实现 `SolFile`，覆盖 AMF0 常用数据类型的读写，确保与原存档格式兼容。

## 下一步建议

1. **平台适配**
   - Windows 平台集成 WebView2 并实现 `IWebViewBridge`。
   - MacCatalyst 平台集成 WKWebView，并处理文件访问权限。
   - 调研 Linux 端方案（WebView2 for Linux、Gtk WebKit、CefGlue 等）。

2. **Flash 模拟器集成**
   - 将 Ruffle WebAssembly 资源嵌入到 MAUI 项目，并在 `IWebViewBridge.InjectFlashPlayerAsync` 中注入加载脚本。
   - 提供本地 SWF 播放器页面，支持文件选择与历史记录。

3. **UI 与交互完善**
   - 迁移设置、收藏夹、历史记录页面。
   - 引入主题与语言资源，确保与原应用体验一致。

4. **测试与 CI**
   - 在 `CrossPlatformCefFlashBrowser.Tests` 中为 SOL 解析、设置服务等关键组件添加单元测试。
   - 建立基础的 GitHub Actions 流水线，执行 .NET 构建与测试。

5. **数据迁移**
   - 实现旧版设置文件、收藏夹与 SOL 存档的导入流程。
   - 提供用户指导文档与迁移工具。

## 参考资源

- [.NET MAUI 官方文档](https://learn.microsoft.com/dotnet/maui/)
- [CommunityToolkit.MVVM](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/)
- [Ruffle Flash 模拟器](https://github.com/ruffle-rs/ruffle)

---

> 本文档会随着项目迭代持续更新，建议在重大结构调整或完成关键里程碑后同步修订。
