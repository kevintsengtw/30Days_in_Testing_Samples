# Day 25 – 範例專案

> [Day 25 – .NET Aspire 整合測試實戰：從 Testcontainers 到 .NET Aspire Testing](https://ithelp.ithome.com.tw/articles/10377197)

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

---

## Aspire 13.1.0 升級說明 (2025-12-27)

### 重大調整概述

本專案已升級至 **Aspire 13.1.0**，該版本引入了全新的 **TLS 終止支持 (TLS Termination Support)** 機制，這是一次重大的安全架構升級。此更新特別解決了之前 Redis 服務自簽 SSL/TLS 憑證配置的問題。

### 核心改進

#### 1. 新的 TLS 終止支持 API

Aspire 13.1.0 引入了統一的 HTTPS 端點配置 API，支持以下服務的內建 TLS：

| 服務     | TLS 支持 | 說明                       |
| -------- | -------- | -------------------------- |
| Redis    | Enabled  | 本專案已啟用開發者自簽憑證 |
| YARP     | Enabled  | 反向代理                   |
| Keycloak | Enabled  | 身份驗證                   |
| Uvicorn  | Enabled  | Python 非同步伺服器        |
| Vite     | Enabled  | 前端開發伺服器             |

#### 2. Redis TLS 配置 (AppHost 中)

```csharp
// 原先 (Aspire 13.0.2 及以前)
var redis = builder.AddRedis("redis")
                   .WithLifetime(ContainerLifetime.Session);

// 新版本 (Aspire 13.1.0+) - 已采用
var redis = builder.AddRedis("redis")
                   .WithLifetime(ContainerLifetime.Session)
                   .WithHttpsDeveloperCertificate();  // 使用開發者 HTTPS 自簽憑證
```

#### 3. 兩層證書 API 系統

Aspire 13.1.0 提供了分層的證書配置 API：

**伺服器端配置 (AppHost)：**

- `WithHttpsDeveloperCertificate()` - 使用 .NET 開發者自簽憑證 [本專案使用]
- `WithHttpsCertificate(certPath, password)` - 使用自訂 PFX/PKCS#12 憑證
- `WithoutHttpsCertificate()` - 禁用 HTTPS (不推薦用於生產)

**用戶端配置 (應用程式中)：**

- `WithDeveloperCertificateTrust()` - 信任 .NET 開發者憑證
- `WithCertificateAuthorityCollection(caCollection)` - 信任自訂 CA 憑證集合
- `WithCertificateTrustScope(trustScope)` - 限定信任範圍

#### 4. 自簽問題的解決方案

**問題背景：**
之前 Aspire 9.4.x → 13.0.0+ 版本跳躍時，Redis 容器的 TLS 自簽憑證未被正確信任，導致連接失敗。

**13.1.0 解決方案：**

- Aspire 自動管理所有服務的 TLS 憑證簽發和信任
- Redis 使用開發者憑證時，應用自動信任該憑證
- 無需手動配置信任存儲或環境變數

**配置示例：**

```csharp
// AppHost 中的 Redis 配置
var redis = builder.AddRedis("redis")
                   .WithLifetime(ContainerLifetime.Session)
                   .WithHttpsDeveloperCertificate();

// 應用程式中可以正常使用
// RedisCacheService 會自動透過 Aspire 的配置連接到 TLS 保護的 Redis
```

### 官方文檔參考

- **完整證書配置指南**：<https://aspire.dev/app-host/certificate-configuration/>
- **Aspire 13.1.0 新功能**：<https://aspire.dev/whats-new/aspire-13-1/>
- **Redis with TLS 配置**：<https://aspire.dev/app-host/certificate-configuration/#configure-redis-with-tls>

### 版本更新清單

已更新所有 Aspire 相關套件從 13.0.2 → 13.1.0：

- `Day25.Api.csproj`
  - `Aspire.Npgsql` 13.0.2 → 13.1.0
  - `Aspire.StackExchange.Redis` 13.0.2 → 13.1.0

- `Day25.AppHost.csproj`
  - `Aspire.AppHost.Sdk` 13.0.2 → 13.1.0
  - `Aspire.Hosting.AppHost` 13.0.2 → 13.1.0
  - `Aspire.Hosting.PostgreSQL` 13.0.2 → 13.1.0
  - `Aspire.Hosting.Redis` 13.0.2 → 13.1.0

- `Day25.Tests.Integration.csproj`
  - `Aspire.Hosting.Testing` 13.0.2 → 13.1.0

---

### 技術亮點

- .NET Aspire 13.1.0 整合測試與 TLS 終止支持
- Redis TLS 自簽憑證配置 - Aspire 13.1.0 新增功能
- PostgreSQL + Dapper 資料存取層設計
- FluentValidation 多層級驗證模式
- 全域例外處理與驗證錯誤統一回應格式
- Container Lifetime 管理 - 測試隔離與自動清理
- Respawn 資料庫狀態重設
- AwesomeAssertions 流暢斷言語法
