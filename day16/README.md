# Day 16 – 範例專案

> [Day 16 – 測試日期與時間：Microsoft.Bcl.TimeProvider 取代 DateTime](https://ithelp.ithome.com.tw/articles/10375821)

---

## 專案結構

```
day16/
├── Day16.Samples.sln
├── src/
│   └── Day16.Core/
│       ├── Day16.Core.csproj
│       ├── GlobalUsings.cs
│       ├── OrderService.cs
│       ├── ScheduleService.cs
│       ├── TimedCache.cs
│       └── TradingService.cs
└── tests/
    └── Day16.Core.Tests/
        ├── AutoFixtureCustomizations.cs
        ├── Day16.Core.Tests.csproj
        ├── FakeTimeProviderExtensions.cs
        ├── GlobalUsings.cs
        ├── OrderServiceAutoFixtureTests.cs
        ├── OrderServiceTests.cs
        ├── ScheduleServiceTests.cs
        ├── TimedCacheTests.cs
        └── TradingServiceTests.cs
```

---

## 專案內容說明

### src/Day16.Core
- **OrderService.cs**：訂單服務，依據當前時間判斷是否可下單，營業時段 9:00~17:00。
- **ScheduleService.cs**：排程服務，根據時間判斷是否該執行排程任務，支援 Cron 與下次執行時間。
- **TimedCache.cs**：帶有過期機制的快取，依據時間自動失效，適合測試快取過期行為。
- **TradingService.cs**：交易服務，判斷是否在交易時段（9:00-11:30、13:00-15:00）。
- **GlobalUsings.cs**：全域 using 設定。

### tests/Day16.Core.Tests
- **OrderServiceTests.cs**：OrderService 的傳統單元測試，手動建立 FakeTimeProvider 驗證營業時段判斷。
- **OrderServiceAutoFixtureTests.cs**：OrderService 的 AutoFixture 測試，展示如何用自動化方式注入 FakeTimeProvider。
- **ScheduleServiceTests.cs**：ScheduleService 的單元測試，驗證排程任務是否該執行。
- **TimedCacheTests.cs**：TimedCache 的單元測試，驗證快取資料的過期與取得行為。
- **TradingServiceTests.cs**：TradingService 的單元測試，驗證不同時間點是否屬於交易時段。
- **AutoFixtureCustomizations.cs**：AutoFixture 的自訂擴充，註冊 FakeTimeProvider 以簡化測試。
- **FakeTimeProviderExtensions.cs**：FakeTimeProvider 的輔助擴充方法，方便設定本地時間。
- **GlobalUsings.cs**：測試專案全域 using 設定。
- **Day16.Core.Tests.csproj**：測試專案組態檔。

---

## 技術重點

- **TimeProvider 注入**：所有服務皆以 TimeProvider 注入，避免硬編碼 DateTime，提升可測試性。
- **FakeTimeProvider**：測試時可精確控制時間，驗證各種時間相關邏輯。
- **AutoFixture 整合**：自動產生 FakeTimeProvider，簡化測試 Arrange 階段。
- **快取與排程測試**：示範如何測試快取過期、排程執行等與時間密切相關的情境。

---

本專案示範如何在 .NET 專案中以 [Microsoft.Bcl.TimeProvider](https://learn.microsoft.com/zh-tw/dotnet/api/system.timeprovider) 取代傳統的 `DateTime`，讓時間相關邏輯更容易測試與注入。

內容包含：
- 以 TimeProvider 取代 DateTime，實現可測試的時間邏輯
- 如何設計與注入時間服務
- 實作可控 FakeTimeProvider 以進行單元測試
- AutoFixture 整合 FakeTimeProvider 的自動化測試
