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
