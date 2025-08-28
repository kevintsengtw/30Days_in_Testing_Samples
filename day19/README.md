# Day 19 – 範例專案

> [Day 19 – 整合測試入門：基礎架構與應用場景](https://ithelp.ithome.com.tw/articles/10376335)

---

## 專案結構

```
day19/
├── Day19.Samples.sln
├── src/
│   └── Day19.WebApplication/
│       ├── appsettings.Development.json
│       ├── appsettings.json
│       ├── Controllers/
│       │   └── ShippersController.cs
│       ├── Data/
│       │   ├── AppDbContext.cs
│       │   ├── Entities/
│       │   │   └── Shipper.cs
│       │   └── ShippingContext.cs
│       ├── Entities/
│       │   └── Shipper.cs
│       ├── GlobalUsings.cs
│       ├── Models/
│       │   ├── Common/
│       │   │   └── ApiResponse.cs
│       │   ├── ShipmentModels.cs
│       │   ├── ShipperCreateParameter.cs
│       │   ├── ShipperOutputModel.cs
│       │   └── SuccessResultOutputModel.cs
│       ├── Program.cs
│       └── Services/
│           ├── IShipperService.cs
│           ├── Level2ExampleServices.cs
│           └── ShipperService.cs
└── tests/
    └── Day19.WebApplication.Integration.Tests/
        ├── Controllers/
        │   └── ShippersControllerTests.cs
        ├── Day19.WebApplication.Integration.Tests.csproj
        ├── Examples/
        │   ├── Level1/
        │   │   ├── BasicApiControllerTests.cs
        │   │   └── ServiceDependentControllerTests.cs
        │   ├── Level2/
        │   │   ├── ServiceDependentControllerTests.cs
        │   │   └── ServiceStubWebApplicationFactory.cs
        │   └── Level3/
        │       └── FullDatabaseIntegrationTests.cs
        ├── GlobalUsings.cs
        ├── Infrastructure/
        │   └── CustomWebApplicationFactory.cs
        ├── Integration/
        │   └── AdvancedShippersControllerTests.cs
        └── IntegrationTestBase.cs
```

---

## 專案內容說明

### src/Day19.WebApplication
- **Controllers/ShippersController.cs**：貨運商 API 控制器，提供查詢、建立、刪除等 RESTful 介面。
- **Data/AppDbContext.cs**：EF Core 資料庫上下文，管理資料存取。
- **Data/Entities/Shipper.cs**、**Entities/Shipper.cs**：貨運商資料表實體。
- **Data/ShippingContext.cs**：進階資料庫情境用 Context。
- **Models/Common/ApiResponse.cs**：API 回應格式共用模型。
- **Models/ShipmentModels.cs**、**ShipperCreateParameter.cs**、**ShipperOutputModel.cs**、**SuccessResultOutputModel.cs**：貨運商與出貨相關資料模型。
- **Services/IShipperService.cs**：貨運商服務介面。
- **Services/ShipperService.cs**：貨運商服務實作，負責資料查詢、建立、刪除等。
- **Services/Level2ExampleServices.cs**：Level2 測試用服務範例。
- **Program.cs**：ASP.NET Core 應用程式進入點。
- **GlobalUsings.cs**：全域 using 設定。

### tests/Day19.WebApplication.Integration.Tests
- **Controllers/ShippersControllerTests.cs**：ShippersController 的整合測試，驗證 API 行為、資料流、狀態碼等。
- **Examples/Level1/BasicApiControllerTests.cs**：Level1 基本 Web API 整合測試，專注於路由、狀態碼、回應格式。
- **Examples/Level1/ServiceDependentControllerTests.cs**：Level1 依賴服務的 Controller 測試。
- **Examples/Level2/ServiceDependentControllerTests.cs**：Level2 Service Stub 整合測試，模擬 Service 依賴。
- **Examples/Level2/ServiceStubWebApplicationFactory.cs**：Level2 測試用 WebApplicationFactory，注入 NSubstitute 服務。
- **Examples/Level3/FullDatabaseIntegrationTests.cs**：Level3 完整資料庫整合測試，驗證資料庫實際操作。
- **Infrastructure/CustomWebApplicationFactory.cs**：自訂 WebApplicationFactory，支援測試環境客製化。
- **Integration/AdvancedShippersControllerTests.cs**：進階 ShippersController 整合測試。
- **IntegrationTestBase.cs**：整合測試基底類別，統一測試初始化、共用邏輯。
- **GlobalUsings.cs**：測試專案全域 using 設定。
- **Day19.WebApplication.Integration.Tests.csproj**：測試專案組態檔。

---

## 技術重點

- **多層級整合測試**：涵蓋無依賴、Service Stub、資料庫等多種測試層次。
- **WebApplicationFactory 客製化**：可依需求注入不同依賴、資料庫、服務。
- **NSubstitute 模擬依賴**：Service Stub 層級測試可精確控制依賴行為。
- **資料庫驗證**：Level3 測試直接操作資料庫，驗證資料流與交易。
- **測試基底類別**：統一初始化、共用程式碼，提升測試維護性。
