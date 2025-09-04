# Day 26 – xUnit 升級指南：從 2.9.x 到 3.x 的轉換

> [Day 26 – xUnit 升級指南：從 2.9.x 到 3.x 的轉換](https://ithelp.ithome.com.tw/articles/10377477)

---

## 專案結構

```text
day26/
├── .gitignore
├── Day26.Samples.sln
├── Day26.Samples.sln.DotSettings.user
├── README.md
├── src/
│   └── Calculator.Core/
│       ├── Calculator.Core.csproj
│       └── Calculator.cs
├── tests/
│   ├── Calculator.Tests.V2/
│   │   ├── Calculator.Tests.V2.csproj
│   │   ├── CalculatorTests.cs
│   │   └── GlobalUsings.cs
│   └── Calculator.Tests.V3/
│       ├── AssemblyInfo.cs
│       ├── Calculator.Tests.V3.csproj
│       ├── CalculatorTests.cs
│       ├── CultureSettingTests.cs
│       ├── DynamicSkipTests.cs
│       ├── Fixtures/
│       │   └── DatabaseAssemblyFixture.cs
│       ├── GlobalUsings.cs
│       ├── RetryMechanismTests.cs
│       ├── TestAttributeAndApiTests.cs
│       ├── TestContextFeatureTests.cs
│       ├── TestOutputIntegrationTests.cs
│       └── xunit.runner.json
```

---

## 專案內容說明

### src/Calculator.Core/
- `Calculator.Core.csproj`：計算器核心程式庫設定。
- `Calculator.cs`：計算器類別，提供加減乘除等基本運算。

### tests/Calculator.Tests.V2/
- `Calculator.Tests.V2.csproj`：xUnit 2.x 測試專案設定。
- `CalculatorTests.cs`：計算器基本功能測試，展示 2.x 寫法。
- `GlobalUsings.cs`：全域 using 設定。

### tests/Calculator.Tests.V3/
- `AssemblyInfo.cs`：組件層級設定。
- `Calculator.Tests.V3.csproj`：xUnit 3.x 測試專案設定。
- `CalculatorTests.cs`：計算器基本功能測試，展示 3.x 新寫法。
- `CultureSettingTests.cs`：多語系文化測試，展示 Culture 設定。
- `DynamicSkipTests.cs`：動態跳過測試，展示 SkipUnless 屬性與平台判斷。
- `Fixtures/DatabaseAssemblyFixture.cs`：組件級 Fixture，管理共享資源。
- `GlobalUsings.cs`：全域 using 設定。
- `RetryMechanismTests.cs`：Retry 機制測試，展示重試行為。
- `TestAttributeAndApiTests.cs`：新屬性與 API 用法展示。
- `TestContextFeatureTests.cs`：TestContext 功能展示。
- `TestOutputIntegrationTests.cs`：TestOutputHelper 與 Console 整合展示。
- `xunit.runner.json`：xUnit 執行器設定檔。

---

## 測試類別簡介

- `CalculatorTests`（V2/V3）：加減乘除等基本功能測試，對比新舊 API 寫法。
- `CultureSettingTests`：多語系文化測試，驗證不同文化下的格式。
- `DynamicSkipTests`：動態跳過測試，根據平台或條件決定是否執行。
- `DatabaseAssemblyFixture`：組件級共享資源管理，展示 IAsyncLifetime 用法。
- `RetryMechanismTests`：測試重試機制，驗證失敗時自動重試。
- `TestAttributeAndApiTests`：新屬性與 API 用法展示。
- `TestContextFeatureTests`：TestContext 功能展示，包含測試資料、狀態、輸出等。
- `TestOutputIntegrationTests`：測試輸出整合，展示 ITestOutputHelper 與 Console。

---

## 執行方式

1. 需安裝 .NET 9.0 SDK。
2. 進入 day26 資料夾，執行：

```powershell
# 清理、建置、執行測試
 dotnet clean
 dotnet build
 dotnet test
```

---

本範例專案展示 xUnit 測試框架從 2.9.x 升級到 3.x 的重點差異，包含 API 變更、新功能、屬性用法、TestContext、Retry、Culture 設定、動態跳過、組件級 Fixture 等。測試專案分為 V2（舊版）與 V3（新版），並以 Calculator 為例，對比各種測試寫法。
