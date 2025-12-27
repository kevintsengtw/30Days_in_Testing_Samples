# Day 28 - TUnit 入門 - 下世代 .NET 測試框架探索

> [Day 28 - TUnit 入門 - 下世代 .NET 測試框架探索](https://ithelp.ithome.com.tw/articles/10377828)

---

## 專案結構

```text
day28/
├── .gitignore
├── Day28.Samples.sln
├── README.md
├── src/
│   └── TUnit.Demo.Core/
│       ├── Calculator.cs
│       ├── EmailValidator.cs
│       ├── GlobalUsings.cs
│       ├── TimeService.cs
│       └── TUnit.Demo.Core.csproj
├── tests/
│   └── TUnit.Demo.Tests/
│       ├── AssertionsExamplesTests.cs
│       ├── CalculatorTests.cs
│       ├── EmailValidatorTests.cs
│       ├── GlobalUsings.cs
│       ├── LifecycleTests.cs
│       ├── TimeProviderTests.cs
│       ├── TUnit.Demo.Tests.csproj
│       └── TUnitAdvancedTests.cs
```

---

## 專案內容說明

### src/TUnit.Demo.Core/

- `Calculator.cs`：基本計算器類別，提供加減乘除等運算。
- `EmailValidator.cs`：電子郵件格式驗證與網域擷取工具。
- `TimeService.cs`：時間服務，支援 TimeProvider/FakeTimeProvider，方便測試時間相關邏輯。
- `GlobalUsings.cs`：全域 using 設定。
- `TUnit.Demo.Core.csproj`：核心程式庫專案設定。

### tests/TUnit.Demo.Tests/

- `AssertionsExamplesTests.cs`：TUnit 斷言語法範例，展示 And/Or 條件組合。
- `CalculatorTests.cs`：Calculator 類別的基本與參數化測試。
- `EmailValidatorTests.cs`：Email 驗證器的格式與例外測試。
- `LifecycleTests.cs`：生命週期管理測試，展示建構式、Dispose、Class/Method Setup。
- `TimeProviderTests.cs`：時間控制與 FakeTimeProvider 測試。
- `TUnitAdvancedTests.cs`：TUnit 進階功能測試，包含時間、條件、折扣等。
- `GlobalUsings.cs`：全域 using 設定。
- `TUnit.Demo.Tests.csproj`：測試專案設定。

---

## 測試類別簡介

- `CalculatorTests`：基本加減乘除與參數化測試。
- `EmailValidatorTests`：Email 格式驗證、網域擷取、例外處理。
- `AssertionsExamplesTests`：TUnit 斷言語法，And/Or 條件組合。
- `LifecycleTests`：建構式、Dispose、Class/Method Setup 生命週期管理。
- `TimeProviderTests`：時間控制、FakeTimeProvider、營業時間判斷。
- `TUnitAdvancedTests`：進階時間測試、條件判斷、特惠邏輯等。

---

## 執行方式

1. 需安裝 .NET 9.0 SDK。
2. 進入 day28 資料夾，執行：

```powershell
# 清理、建置、執行測試
 dotnet clean
 dotnet build
 dotnet test
```

### 執行測試專案

在 Terminal 進入 `/day28/tests/TUnit.Demo.Tests/` 測試專案的目錄

然後執行 `dotnet run` 就可以直接執行測試，以下為執行結果的顯示內容

```text
████████╗██╗   ██╗███╗   ██╗██╗████████╗
╚══██╔══╝██║   ██║████╗  ██║██║╚══██╔══╝
   ██║   ██║   ██║██╔██╗ ██║██║   ██║
   ██║   ██║   ██║██║╚██╗██║██║   ██║
   ██║   ╚██████╔╝██║ ╚████║██║   ██║
   ╚═╝    ╚═════╝ ╚═╝  ╚═══╝╚═╝   ╚═╝

   TUnit v0.57.24.0 | 64-bit | Microsoft Windows 10.0.26100 | win-x64 | .NET 9.0.8 | Microsoft Testing Platform v1.8.4

   Engine Mode: SourceGenerated

建構式：建立 Calculator 實例
建構式：建立 Calculator 實例
執行測試：Multiply_基本測試
執行測試：Add_基本測試
Dispose：清理資源
Dispose：清理資源
2. 建構式執行
1. Before(Class) 執行
3. Before(Test) 執行
4. 測試方法執行
5. After(Test) 執行
6. Dispose 執行
7. After(Class) 執行

[✓52/x0/↓0] TUnit.Demo.Tests.dll (net9.0|x64)                                   (0s) 
  資料庫測試2_不並行執行                                                           (0s) 
資料庫初始化完成
測試準備：清理資料庫狀態
測試準備：清理資料庫狀態

測試回合摘要： 成功! - bin\Debug\net9.0\TUnit.Demo.Tests.dll (net9.0|x64)
  total: 53
  failed: 0
  succeeded: 53
  skipped: 0
  duration: 550ms
```

---

本範例專案介紹 TUnit 測試框架的基本用法與進階特性，包含斷言語法、生命週期管理、參數化測試、時間控制、FakeTimeProvider、Email 驗證、進階條件組合等。測試專案涵蓋多種常見測試情境，並對比 TUnit 與傳統 xUnit/NUnit 的差異。

---

## 套件版本說明

### 版本相依性鏈鎖 - 核心限制

本專案所有套件版本的選擇均受 **TUnit 0.58.0** 所依賴的 **Microsoft.Testing.Platform 版本**限制。此為升級決策的根本原因。

#### 版本相依性關係圖

```text
TUnit 0.58.0
    ↓ 依賴
Microsoft.Testing.Platform (1.8.x)
    ↑ 被依賴
├─ Microsoft.Testing.Extensions.CodeCoverage (需 1.8.x) → 最新可用 18.0.6
├─ Microsoft.Testing.Extensions.TrxReport (需 1.8.x) → 最新可用 1.8.5
└─ Microsoft.Testing.Extensions.CodeCoverage 18.1.0+ (需 2.0.x+) → 無法使用
└─ Microsoft.Testing.Extensions.TrxReport 2.0.0+ (需 2.0.2+) → 無法使用
```

### Microsoft.Testing.Extensions.CodeCoverage 版本選擇

本專案採用 **Microsoft.Testing.Extensions.CodeCoverage 18.0.6**，而非最新的 18.1.0：

| 特性                           | 18.0.6                 | 18.1.0                                         |
| ------------------------------ | ---------------------- | ---------------------------------------------- |
| **所需 Testing.Platform 版本** | 1.8.x                  | 2.0.x 或更高                                   |
| **與 TUnit 0.58.0 相容**       | 完全相容               | 不相容                                         |
| **測試結果**                   | 全部 53 測試通過       | TypeLoadException：無法載入 IDataConsumer 型別 |
| **穩定性**                     | 針對 .NET 9.0 環境優化 | 需要升級 TUnit 才可使用                        |

### Microsoft.Testing.Extensions.TrxReport 版本選擇

本專案採用 **Microsoft.Testing.Extensions.TrxReport 1.8.5**，而非更新的 1.9.0 或 2.0.0+：

| 特性                      | 1.8.5            | 1.9.0+                       | 2.0.0+             |
| ------------------------- | ---------------- | ---------------------------- | ------------------ |
| **所需 Testing.Platform** | 1.8.x            | 1.8.x（但引入 API 變更）     | 2.0.2+             |
| **與 TUnit 0.58.0 相容**  | 完全相容         | 相容但測試行為改變           | 不相容             |
| **測試結果**              | 全部 53 測試通過 | 1 個測試失敗（user 為 null） | NuGet 版本衝突拒絕 |

**核心限制分析：**

- **TUnit 0.58.0** 依賴的 Microsoft.Testing.Platform 版本較舊（1.8.x）
- **CodeCoverage 18.1.0+** 要求 Microsoft.Testing.Platform 2.0.x 或更高，造成版本衝突
- **TrxReport 2.0.0+** 要求 Microsoft.Testing.Platform >= 2.0.2，NuGet 明確拒絕版本降級，導致無法使用
- **解決方案：** 保持在 1.8.x 版本直到 TUnit 升級到支援新版本 Microsoft.Testing.Platform 為止

**版本差異詳解：**

- **1.8.5 vs 1.9.0：** 1.9.0 引入新的 API 或行為變更，導致生命週期管理上的差異
- **1.8.5 vs 2.0.0+：** 版本跨度大，2.0.0+ 明確聲明需要 Microsoft.Testing.Platform 2.0.2+，產生不可調和的相依性衝突
