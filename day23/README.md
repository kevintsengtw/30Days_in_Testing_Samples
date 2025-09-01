# Day 23 – 範例專案

> [Day 23 – 整合測試實戰：WebApi 服務的整合測試](https://ithelp.ithome.com.tw/articles/10376873)

---

## 專案結構

```text
day23/
├── Day23.Samples.sln                        # 方案檔
├── .gitignore                               # 忽略設定
├── src/
│   ├── Day23.WebApi/
│   │   ├── Day23.WebApi.csproj              # WebApi 專案 csproj
│   │   ├── GlobalUsings.cs                  # 全域 using 設定
│   │   ├── Program.cs                       # ASP.NET Core 啟動程式
│   │   ├── Controllers/
│   │   │   ├── ProductsController.cs        # 產品 API 控制器
│   │   │   └── HealthController.cs          # 健康檢查 API 控制器
│   │   ├── Middleware/
│   │   │   ├── GlobalExceptionHandler.cs    # 全域異常處理中介軟體
│   │   │   └── FluentValidationExceptionHandler.cs # FluentValidation 專用異常處理
│   │   ├── appsettings.json                 # 主要設定檔
│   │   ├── appsettings.Development.json     # 開發環境設定檔
│   │   └── Properties/
│   │       └── launchSettings.json          # 啟動設定
│   ├── Day23.Application/
│   │   ├── Day23.Application.csproj         # 應用層 csproj
│   │   ├── GlobalUsings.cs                  # 全域 using 設定
│   │   ├── Abstractions/
│   │   │   ├── IProductRepository.cs        # 產品儲存庫介面
│   │   │   ├── ICacheService.cs             # 快取服務介面
│   │   │   └── CacheKeys.cs                 # 快取鍵值管理
│   │   ├── Dtos/
│   │   │   ├── ProductResponse.cs           # 產品回應 DTO
│   │   │   ├── ProductCreateRequest.cs      # 建立產品請求 DTO
│   │   │   ├── ProductUpdateRequest.cs      # 更新產品請求 DTO
│   │   │   └── PagedResult.cs               # 分頁結果 DTO
│   │   ├── Services/
│   │   │   ├── ProductService.cs            # 產品服務實作
│   │   │   └── IProductService.cs           # 產品服務介面
│   │   └── Validation/
│   │       ├── ProductCreateValidator.cs    # 建立產品請求驗證器
│   │       └── ProductUpdateValidator.cs    # 更新產品請求驗證器
│   ├── Day23.Domain/
│   │   ├── Day23.Domain.csproj              # 領域層 csproj
│   │   ├── GlobalUsings.cs                  # 全域 using 設定
│   │   └── Product.cs                       # 產品領域模型
│   └── Day23.Infrastructure/
│       ├── Day23.Infrastructure.csproj      # 基礎設施層 csproj
│       ├── GlobalUsings.cs                  # 全域 using 設定
│       ├── Repositories/
│       │   └── ProductRepository.cs         # 產品儲存庫實作
│       ├── Caching/
│       │   └── RedisCacheService.cs         # Redis 快取服務實作
│       └── Database/
│           ├── DbConnectionFactory.cs       # 資料庫連線工廠
│           └── Sql/
│               └── CreateTables.sql         # 資料表建立 SQL 指令碼
├── tests/
│   └── Day23.Integration.Tests/
│       ├── Day23.Integration.Tests.csproj   # 測試專案 csproj
│       ├── GlobalUsings.cs                  # 測試專案全域 using 設定
│       ├── Controllers/
│       │   ├── ProductsControllerTests.cs   # 產品 API 整合測試
│       │   ├── HealthControllerTests.cs     # 健康檢查 API 測試
│       │   └── ExceptionHandlerTests.cs     # 異常處理測試
│       ├── Infrastructure/
│       │   ├── IntegrationTestBase.cs       # 整合測試基底類別
│       │   ├── TestWebApplicationFactory.cs # 測試用 WebApplicationFactory
│       │   ├── DatabaseManager.cs           # 測試用資料庫管理工具
│       │   ├── IntegrationTestCollection.cs # 測試集合定義
│       │   └── TestHelpers.cs               # 測試輔助工具
│       └── SqlScripts/
│           └── Tables/
│               └── CreateProductsTable.sql  # 產品資料表建立 SQL 指令碼
```

---

## 專案內容簡介

### 1. `src/Day23.WebApi/`
- **Day23.WebApi.csproj**：WebApi 專案描述與依賴。
- **GlobalUsings.cs**：全域 using 設定。
- **Program.cs**：ASP.NET Core 啟動程式，註冊 DI、Middleware、Swagger 等。
- **Controllers/ProductsController.cs**：產品 API，CRUD 與分頁查詢。
- **Controllers/HealthController.cs**：健康檢查 API。
- **Middleware/GlobalExceptionHandler.cs**：全域異常處理，統一回應格式。
- **Middleware/FluentValidationExceptionHandler.cs**：FluentValidation 驗證失敗專用異常處理。
- **appsettings.json / appsettings.Development.json**：環境設定。
- **Properties/launchSettings.json**：啟動設定。

### 2. `src/Day23.Application/`
- **Day23.Application.csproj**：應用層專案描述與依賴。
- **GlobalUsings.cs**：全域 using 設定。
- **Abstractions/IProductRepository.cs**：產品儲存庫介面，定義資料存取操作。
- **Abstractions/ICacheService.cs**：快取服務介面。
- **Abstractions/CacheKeys.cs**：快取鍵值管理。
- **Dtos/ProductResponse.cs**：產品回應 DTO。
- **Dtos/ProductCreateRequest.cs**：建立產品請求 DTO。
- **Dtos/ProductUpdateRequest.cs**：更新產品請求 DTO。
- **Dtos/PagedResult.cs**：分頁結果 DTO。
- **Services/ProductService.cs**：產品服務實作，負責業務邏輯。
- **Services/IProductService.cs**：產品服務介面。
- **Validation/ProductCreateValidator.cs**：建立產品請求驗證器。
- **Validation/ProductUpdateValidator.cs**：更新產品請求驗證器。

### 3. `src/Day23.Domain/`
- **Day23.Domain.csproj**：領域層專案描述。
- **GlobalUsings.cs**：全域 using 設定。
- **Product.cs**：產品領域模型。

### 4. `src/Day23.Infrastructure/`
- **Day23.Infrastructure.csproj**：基礎設施層專案描述。
- **GlobalUsings.cs**：全域 using 設定。
- **Repositories/ProductRepository.cs**：產品儲存庫實作，資料庫 CRUD。
- **Caching/RedisCacheService.cs**：Redis 快取服務實作。
- **Database/DbConnectionFactory.cs**：資料庫連線工廠。
- **Database/Sql/CreateTables.sql**：資料表建立 SQL 指令碼。

### 5. `tests/Day23.Integration.Tests/`
- **Day23.Integration.Tests.csproj**：測試專案描述，引用 Testcontainers、xUnit、AwesomeAssertions 等。
- **GlobalUsings.cs**：測試專案全域 using 設定。
- **Controllers/ProductsControllerTests.cs**：產品 API 整合測試，涵蓋 CRUD、分頁、驗證、快取等。
- **Controllers/HealthControllerTests.cs**：健康檢查 API 測試。
- **Controllers/ExceptionHandlerTests.cs**：異常處理測試，驗證 ProblemDetails 回應。
- **Infrastructure/IntegrationTestBase.cs**：整合測試基底類別，負責初始化與清理。
- **Infrastructure/TestWebApplicationFactory.cs**：測試用 WebApplicationFactory，啟動 PostgreSQL、Redis 容器。
- **Infrastructure/DatabaseManager.cs**：測試用資料庫管理工具，初始化/清理資料表。
- **Infrastructure/IntegrationTestCollection.cs**：測試集合定義，所有整合測試共享容器。
- **Infrastructure/TestHelpers.cs**：測試輔助工具，JSON 處理、測試資料產生等。
- **SqlScripts/Tables/CreateProductsTable.sql**：產品資料表建立 SQL 指令碼。

---

## 測試專案各測試類別說明

- **ProductsControllerTests.cs**：針對產品 API 進行整合測試，涵蓋新增、查詢、分頁、更新、刪除、快取、驗證等情境。
- **HealthControllerTests.cs**：健康檢查 API 測試，驗證服務狀態與回應內容。
- **ExceptionHandlerTests.cs**：異常處理測試，驗證 404、驗證失敗等回應格式。
- **Infrastructure/IntegrationTestBase.cs**：整合測試基底類別，負責測試前後的資料庫初始化與清理。
- **Infrastructure/TestWebApplicationFactory.cs**：啟動 PostgreSQL、Redis 容器，建立測試用 Web API。
- **Infrastructure/DatabaseManager.cs**：資料庫結構初始化、資料清理。
- **Infrastructure/IntegrationTestCollection.cs**：定義測試集合，讓所有測試共享容器資源。
- **Infrastructure/TestHelpers.cs**：JSON 處理、測試資料產生等輔助工具。

---

此專案展示如何以 Testcontainers 建立 WebApi 整合測試環境，並結合 PostgreSQL、Redis 進行端到端驗證。
