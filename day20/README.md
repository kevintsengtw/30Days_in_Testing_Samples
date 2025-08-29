# Day 20 – 範例專案

> [ay 20 – Testcontainers 初探：使用 Docker 架設測試環境](https://ithelp.ithome.com.tw/articles/10376401)

---

## 專案結構

```text

├── Day20.Samples.sln                # 方案檔
├── verify-environment.ps1           # 驗證 Docker 測試環境的 PowerShell 腳本
├── src/
│   └── Day20.Core/
│       ├── Day20.Core.csproj        # 核心專案 csproj
│       ├── GlobalUsings.cs          # 全域 using 設定
│       ├── Data/
│       │   └── UserDbContext.cs     # Entity Framework DbContext，支援多種資料庫
│       ├── Models/
│       │   ├── User.cs              # 使用者資料模型
│       │   └── UserRequests.cs      # 使用者請求 DTO
│       └── Services/
│           ├── CacheService.cs      # Redis 快取服務介面與實作
│           ├── IExternalApiService.cs # 外部 API 服務介面
│           ├── IUserService.cs      # 使用者服務介面
│           ├── SqlUserService.cs    # SQL 資料庫實作的使用者服務
│           └── Implementations/
│               └── ExternalApiService.cs # 外部 API 服務實作
├── tests/
│   └── Day20.Core.Integration.Tests/
│       ├── Day20.Core.Integration.Tests.csproj # 測試專案 csproj
│       ├── GlobalUsings.cs          # 測試專案全域 using 設定
│       └── Integration/
│           ├── PostgreSqlIntegrationTests.cs   # PostgreSQL 整合測試
│           ├── SqlServerIntegrationTests.cs    # SQL Server 整合測試
│           ├── RedisIntegrationTests.cs        # Redis 整合測試
│           ├── UserServicePostgreSqlTests.cs   # PostgreSQL 下 UserService 整合測試
│           ├── UserServiceSqlServerTests.cs    # SQL Server 下 UserService 整合測試
│           └── WireMockIntegrationTests.cs     # WireMock 外部 API 模擬整合測試
```

---

## 專案內容簡介

### 1. `src/Day20.Core/`
- **Day20.Core.csproj**：專案描述與套件依賴，支援 EF Core、Npgsql、Redis。
- **GlobalUsings.cs**：全域 using，簡化程式碼。
- **Data/UserDbContext.cs**：EF Core DbContext，支援 PostgreSQL/SQL Server，定義 User 資料表結構。
- **Models/User.cs**：使用者資料模型，含驗證屬性。
- **Models/UserRequests.cs**：建立/更新使用者的請求 DTO。
- **Services/CacheService.cs**：定義 ICacheService 介面與 RedisCacheService 實作。
- **Services/IExternalApiService.cs**：外部 API 服務介面，定義 Email 驗證與地理位置查詢。
- **Services/IUserService.cs**：使用者服務介面，CRUD 與查詢。
- **Services/SqlUserService.cs**：以 SQL 資料庫為基礎的 UserService 實作。
- **Services/Implementations/ExternalApiService.cs**：外部 API 服務實作，實際呼叫 HTTP API。

### 2. `tests/Day20.Core.Integration.Tests/`
- **Day20.Core.Integration.Tests.csproj**：測試專案描述，引用 Testcontainers、xUnit、WireMock 等。
- **GlobalUsings.cs**：測試專案全域 using 設定。
- **Integration/PostgreSqlIntegrationTests.cs**：
  - 利用 Testcontainers 啟動 PostgreSQL 容器，測試資料庫連線、資料操作。
- **Integration/SqlServerIntegrationTests.cs**：
  - 利用 Testcontainers 啟動 SQL Server 容器，測試資料庫連線、資料操作。
- **Integration/RedisIntegrationTests.cs**：
  - 利用 Testcontainers 啟動 Redis 容器，測試快取功能（如字串儲存/讀取、物件序列化等）。
- **Integration/UserServicePostgreSqlTests.cs**：
  - PostgreSQL 下，針對 SqlUserService 進行 CRUD 與驗證測試。
- **Integration/UserServiceSqlServerTests.cs**：
  - SQL Server 下，針對 SqlUserService 進行 CRUD 與驗證測試。
- **Integration/WireMockIntegrationTests.cs**：
  - 利用 WireMock Testcontainer 模擬外部 API，測試 ExternalApiService 的 Email 驗證與地理位置查詢。

### 3. 其他
- **verify-environment.ps1**：一鍵檢查 Docker 與測試環境狀態的 PowerShell 腳本。
- **.gitignore**：忽略 bin/obj、Docker 產生檔案等。

---

## 技術重點
- 使用 Testcontainers 自動啟動/銷毀 Docker 容器，實現跨資料庫、快取、API 整合測試
- 支援 PostgreSQL、SQL Server、Redis、WireMock
- 測試類別皆實作 IAsyncLifetime，確保容器與資源正確初始化與釋放
- 適合 CI/CD pipeline、跨平台測試、真實環境驗證

---

本範例專案示範如何利用 Testcontainers 在 .NET 測試中自動啟動 Docker 容器，進行跨資料庫（PostgreSQL、SQL Server）、Redis 及外部 API（WireMock）整合測試。適合想學習自動化測試環境、資料庫與快取整合測試的工程師。