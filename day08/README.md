# Day 08：範例專案

[Day 08：測試輸出與記錄 - xUnit ITestOutputHelper 與 ILogger](https://ithelp.ithome.com.tw/articles/10374711)

---

## 專案結構

```text
day08/
├── src/
│   └── Day08.Core/
│       ├── Day08.Core.csproj
│       ├── bin/
│       ├── GlobalUsings.cs
│       ├── Interface/
│       ├── Logging/
│       │   └── AbstractLogger.cs
│       ├── Models/
│       ├── obj/
│       └── Services/
│           ├── DataProcessingResult.cs
│           ├── DataProcessor.cs
│           ├── OrderProcessingService.cs
│           ├── OrderProcessor.cs
│           ├── PriceCalculationResult.cs
│           └── ProductService.cs
├── tests/
│   └── Day08.Core.Tests/
│       ├── bin/
│       ├── Day08.Core.Tests.csproj
│       ├── GlobalUsings.cs
│       ├── Integration/
│       ├── LoggerTests/
│       │   ├── AsyncLoggingTests.cs
│       │   ├── OrderProcessingAdvancedTests.cs
│       │   ├── OrderProcessingServiceTests.cs
│       │   └── PaymentServiceTests.cs
│       ├── Logging/
│       ├── obj/
│       └── TestOutputHelper/
│           ├── DiagnosticTestBase.cs
│           ├── PerformanceTests.cs
│           ├── ProductServiceDiagnosticTests.cs
│           ├── ProductServiceTests.cs
│           └── StructuredOutputTests.cs
└── Day08.Samples.sln
```

---

## 專案內容簡介

### src/Day08.Core/
- **Logging/AbstractLogger.cs**：自訂抽象 logger，支援多種記錄方式。
- **Services/**：資料處理、訂單與產品服務等核心業務邏輯。

### tests/Day08.Core.Tests/

**Logging/**：
  - `CompositeLogger.cs`：組合 Logger，可同時將日誌導向多個 Logger 實例。
  - `ConcurrentTestLogger.cs`：支援多執行緒安全的測試 Logger，適合並行測試收集日誌。
  - `LogEntry.cs`：日誌項目資料結構，包含層級、訊息、狀態與例外。
  - `NoOpDisposable.cs`：無操作的 IDisposable 實作，供 Logger 範圍用。
  - `TestLogger.cs`：單元測試專用 Logger，支援日誌收集與驗證。
  - `XUnitLogger.cs`：xUnit 測試專用 Logger，將日誌訊息導向 ITestOutputHelper，便於測試輸出。

**TestOutputHelper/**：
  - `ProductServiceTests.cs`：基礎 ITestOutputHelper 用法，於測試過程中輸出診斷與驗證資訊。
  - `ProductServiceDiagnosticTests.cs`：商品服務診斷測試，繼承 DiagnosticTestBase，結合測試上下文與複雜資料輸出。
  - `PerformanceTests.cs`：效能測試，記錄處理時間、各階段檢查點，協助分析瓶頸。
  - `StructuredOutputTests.cs`：結構化輸出範例，將測試資料、過程與結果以分段、格式化方式輸出，提升可讀性。
  - `DiagnosticTestBase.cs`：診斷測試基底類別，提供統一的測試上下文與資料記錄輔助方法。

**LoggerTests/**：
  - `AsyncLoggingTests.cs`：非同步記錄測試，驗證服務在 async/await 流程下的日誌正確性與訊息內容。
  - `OrderProcessingServiceTests.cs`：以抽象 logger 驗證訂單處理服務的記錄行為，涵蓋正常與異常流程。
  - `OrderProcessingAdvancedTests.cs`：進階訂單處理測試，結合多種 logger（如 xUnit 輸出、CompositeLogger）驗證記錄與測試輸出。
  - `PaymentServiceTests.cs`：結構化記錄測試，驗證付款服務記錄內容、敏感資訊遮蔽與錯誤訊息。

**Integration/**：
  - `OrderProcessingIntegrationTests.cs`：訂單處理整合測試，結合 DI、xUnit Logger，驗證完整業務流程與日誌。
  - `XUnitLoggerGeneric.cs`：泛型 xUnit Logger 實作，將日誌導向 ITestOutputHelper，支援類別分類與範圍。
  - `XUnitLoggerProvider.cs`：xUnit Logger 提供者，整合 Microsoft.Extensions.Logging，供整合測試註冊使用。

---

此資料夾為 Day 08 的範例專案，重點在於 xUnit 測試輸出、診斷與記錄技巧。
