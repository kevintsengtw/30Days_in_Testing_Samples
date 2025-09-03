# Day 25 – 範例專案

> [Day 25 – .NET Aspire 整合測試實戰：從 Testcontainers 到 .NET Aspire Testing](https://ithelp.ithome.com.tw/articles/10377197)

---

## 專案結構

```text
day25/
├── .gitignore
├── Day25.Samples.sln
├── README.md
├── src/
│   ├── Day25.Api/
│   │   ├── appsettings.Development.json
│   │   ├── appsettings.json
│   │   ├── Configuration/
│   │   │   ├── ApiConfiguration.cs
│   │   │   └── ValidationFilter.cs
│   │   ├── Controllers/
│   │   │   ├── HealthController.cs
│   │   │   └── ProductsController.cs
│   │   ├── ExceptionHandlers/
│   │   │   ├── FluentValidationExceptionHandler.cs
│   │   │   └── GlobalExceptionHandler.cs
│   │   ├── GlobalUsings.cs
│   │   ├── Day25.Api.csproj
│   │   ├── Program.cs
│   │   └── Properties/
│   │       └── launchSettings.json
│   ├── Day25.AppHost/
│   │   ├── Day25.AppHost.csproj
│   │   └── Program.cs
│   ├── Day25.Application/
│   │   ├── Day25.Application.csproj
│   │   ├── DependencyInjection.cs
│   │   ├── DTOs/
│   │   │   ├── ProductDTOs.cs
│   │   │   └── PagedResult.cs
│   │   ├── GlobalUsings.cs
│   │   ├── Models/
│   │   └── Services/
│   │       ├── IProductService.cs
│   │       └── ProductService.cs
│   ├── Day25.Domain/
│   │   ├── Day25.Domain.csproj
│   │   ├── Exceptions/
│   │   │   └── ProductNotFoundException.cs
│   │   ├── GlobalUsings.cs
│   │   └── Product.cs
│   └── Day25.Infrastructure/
│       ├── Caching/
│       │   ├── ICacheService.cs
│       │   └── RedisCacheService.cs
│       ├── Data/
│       │   ├── DapperTypeMapping.cs
│       │   ├── IProductRepository.cs
│       │   ├── Mapping/
│       │   │   └── StringExtensions.cs
│       │   └── ProductRepository.cs
│       ├── Day25.Infrastructure.csproj
│       ├── DependencyInjection.cs
│       ├── GlobalUsings.cs
│       ├── Services/
│       └── Validation/
│           ├── ProductCreateRequestValidator.cs
│           └── ProductUpdateRequestValidator.cs
├── tests/
│   └── Day25.Tests.Integration/
│       ├── Controllers/
│       │   ├── HealthControllerTests.cs
│       │   └── ProductsControllerTests.cs
│       ├── Day25.Tests.Integration.csproj
│       ├── GlobalUsings.cs
│       ├── Infrastructure/
│       │   ├── AspireAppFixture.cs
│       │   ├── DatabaseManager.cs
│       │   ├── IntegrationTestBase.cs
│       │   ├── IntegrationTestCollection.cs
│       │   └── TestHelpers.cs
│       ├── SqlScripts/
│       │   └── Tables/
│       │       └── CreateProductsTable.sql
│       └── VerifyAspireContainers.cs
```

---

## 專案內容說明

### src/Day25.Api/
- `appsettings.Development.json`、`appsettings.json`：API 設定檔。
- `Configuration/ApiConfiguration.cs`：API 服務相關設定。
- `Configuration/ValidationFilter.cs`：全域驗證過濾器。
- `Controllers/HealthController.cs`：健康檢查 API。
- `Controllers/ProductsController.cs`：產品 CRUD 與查詢 API。
- `ExceptionHandlers/FluentValidationExceptionHandler.cs`：FluentValidation 例外處理。
- `ExceptionHandlers/GlobalExceptionHandler.cs`：全域例外處理。
- `GlobalUsings.cs`：全域 using 設定。
- `Day25.Api.csproj`：API 專案設定。
- `Program.cs`：API 入口點。
- `Properties/launchSettings.json`：本機啟動設定。

### src/Day25.AppHost/
- `Day25.AppHost.csproj`：Aspire AppHost 專案設定。
- `Program.cs`：Aspire 分散式應用程式入口。

### src/Day25.Application/
- `Day25.Application.csproj`：應用層專案設定。
- `DependencyInjection.cs`：DI 註冊。
- `DTOs/ProductDTOs.cs`、`PagedResult.cs`：產品資料傳輸物件與分頁結果模型。
- `GlobalUsings.cs`：全域 using 設定。
- `Models/`：應用層模型。
- `Services/IProductService.cs`、`ProductService.cs`：產品業務邏輯介面與實作。

### src/Day25.Domain/
- `Day25.Domain.csproj`：網域層專案設定。
- `Exceptions/ProductNotFoundException.cs`：產品找不到例外。
- `GlobalUsings.cs`：全域 using 設定。
- `Product.cs`：產品網域模型。

### src/Day25.Infrastructure/
- `Caching/ICacheService.cs`、`RedisCacheService.cs`：快取服務介面與 Redis 實作。
- `Data/DapperTypeMapping.cs`：Dapper 型別對應設定。
- `Data/IProductRepository.cs`、`ProductRepository.cs`：產品資料存取介面與實作。
- `Data/Mapping/StringExtensions.cs`：字串擴充方法。
- `Day25.Infrastructure.csproj`：基礎建設層專案設定。
- `DependencyInjection.cs`：DI 註冊。
- `GlobalUsings.cs`：全域 using 設定。
- `Services/`：基礎建設服務。
- `Validation/ProductCreateRequestValidator.cs`、`ProductUpdateRequestValidator.cs`：產品建立/更新驗證。

### tests/Day25.Tests.Integration/
- `Controllers/HealthControllerTests.cs`：API 健康檢查整合測試。
- `Controllers/ProductsControllerTests.cs`：API 產品 CRUD 與查詢整合測試。
- `Day25.Tests.Integration.csproj`：整合測試專案設定。
- `GlobalUsings.cs`：全域 using 設定。
- `Infrastructure/AspireAppFixture.cs`：Aspire 測試環境管理。
- `Infrastructure/DatabaseManager.cs`：資料庫測試輔助。
- `Infrastructure/IntegrationTestBase.cs`：整合測試基底類別。
- `Infrastructure/IntegrationTestCollection.cs`：測試集合定義。
- `Infrastructure/TestHelpers.cs`：測試輔助工具。
- `SqlScripts/Tables/CreateProductsTable.sql`：建立產品資料表 SQL。
- `VerifyAspireContainers.cs`：驗證 Aspire 容器啟動與服務可用性。

---

## 測試類別簡介

- `ProductsControllerTests`：API 產品 CRUD、查詢、分頁、驗證、異常處理等整合測試。
- `HealthControllerTests`：API 健康檢查與存活檢查測試。
- `VerifyAspireContainers`：驗證 Aspire 測試環境下 PostgreSQL、Redis 等容器服務可用性。
- `IntegrationTestBase`：所有整合測試的基底，負責資料庫初始化與清理。
- `AspireAppFixture`：負責啟動、管理 Aspire 測試應用與 HTTP 客戶端。

---

## 執行方式

1. 需安裝 .NET 9.0 SDK 與 Docker。
2. 進入 day25 資料夾，執行：

```powershell
dotnet clean
dotnet build
dotnet test
```

---

本範例專案展示如何以 .NET Aspire Testing 進行分散式應用的整合測試，結合 PostgreSQL、Redis、API、資料存取層、快取、例外處理等，並對比 Testcontainers 與 .NET Aspire Testing 的實務差異。測試專案涵蓋 API、資料庫、快取、交易、隔離等主題。