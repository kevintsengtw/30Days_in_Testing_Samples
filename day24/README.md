# Day 24 - 範例專案

> [Day 24 - .NET Aspire Testing 入門基礎介紹](https://ithelp.ithome.com.tw/articles/10377071)

---

## 專案結構

```text
day24/
├── .gitignore
├── Day24.Samples.sln
├── Day24.Samples.sln.DotSettings.user
├── src/
│   ├── BookStore.AppHost/
│   │   ├── BookStore.AppHost.csproj
│   │   └── Program.cs
│   └── BookStore.Core/
│       ├── BookStore.Core.csproj
│       ├── Data/
│       │   └── BookStoreDbContext.cs
│       ├── GlobalUsings.cs
│       ├── Migrations/
│       │   ├── 20241201000000_InitialCreate.cs
│       │   └── BookStoreDbContextModelSnapshot.cs
│       ├── Models/
│       │   └── Book.cs
│       ├── Repositories/
│       │   ├── EfCoreBookRepository.cs
│       │   └── IBookRepository.cs
│       └── Services/
│           ├── BookService.cs
│           └── IBookService.cs
├── tests/
│   └── BookStore.Tests/
│       ├── BookStore.Tests.csproj
│       ├── GlobalUsings.cs
│       ├── Helpers/
│       │   ├── DatabaseTestHelper.cs
│       │   ├── QueryResult.cs
│       │   └── TestDataSeeder.cs
│       ├── Infrastructure/
│       │   ├── AspireAppCollectionDefinition.cs
│       │   └── AspireAppFixture.cs
│       ├── Integration/
│       │   ├── BookServiceTests.cs
│       │   ├── BookStoreDbTests.cs
│       │   ├── DatabaseFeatureTests.cs
│       │   ├── EfCoreBookRepositoryTests.cs
│       │   ├── ErrorHandlingTests.cs
│       │   ├── SimplifiedBookStoreTests.cs
│       │   ├── TestIsolationTests.cs
│       │   └── TransactionTests.cs
│       └── Models/
│           └── BookSummary.cs
```

---

## 專案內容說明

### src/BookStore.AppHost/

- `BookStore.AppHost.csproj`：主機專案設定檔。
- `Program.cs`：Aspire 分散式應用程式入口，註冊 SQL Server 容器與資料庫。

### src/BookStore.Core/

- `BookStore.Core.csproj`：核心程式庫設定。
- `Data/BookStoreDbContext.cs`：EF Core 資料庫上下文，定義 Books 資料表。
- `GlobalUsings.cs`：全域 using 設定。
- `Migrations/20241201000000_InitialCreate.cs`、`BookStoreDbContextModelSnapshot.cs`：資料庫遷移腳本與快照。
- `Models/Book.cs`：書籍實體類別，包含標題、作者、價格、出版日期等欄位。
- `Repositories/EfCoreBookRepository.cs`：EF Core 書籍資料存取實作，CRUD 與查詢。
- `Repositories/IBookRepository.cs`：書籍資料存取介面。
- `Services/BookService.cs`：業務邏輯實作，包含建立、查詢、更新、刪除、推薦等功能。
- `Services/IBookService.cs`：業務邏輯介面。

### tests/BookStore.Tests/

- `BookStore.Tests.csproj`：測試專案設定。
- `GlobalUsings.cs`：測試全域 using 設定。
- `Helpers/DatabaseTestHelper.cs`：資料庫測試輔助工具。
- `Helpers/QueryResult.cs`：SQL 查詢結果模型。
- `Helpers/TestDataSeeder.cs`：測試資料播種工具。
- `Infrastructure/AspireAppCollectionDefinition.cs`：Aspire 測試集合定義。
- `Infrastructure/AspireAppFixture.cs`：Aspire 測試環境管理。
- `Integration/BookServiceTests.cs`：BookService 業務邏輯測試，驗證建立、查詢、異常處理。
- `Integration/BookStoreDbTests.cs`：資料庫測試（目前空檔案，預留擴充）。
- `Integration/DatabaseFeatureTests.cs`：資料庫遷移、SQL 查詢、欄位結構驗證。
- `Integration/EfCoreBookRepositoryTests.cs`：Repository CRUD、查詢、異常處理、資料更新與刪除測試。
- `Integration/ErrorHandlingTests.cs`：錯誤處理與診斷、健康檢查測試。
- `Integration/SimplifiedBookStoreTests.cs`：簡化版模型、DbContext、資料播種展示。
- `Integration/TestIsolationTests.cs`：交易隔離、唯一資料測試。
- `Integration/TransactionTests.cs`：交易回滾、並發一致性測試。
- `Models/BookSummary.cs`：測試用書籍摘要模型。

---

## 測試類別簡介

- `BookServiceTests`：驗證業務邏輯正確性，包含有效/無效資料建立、例外處理。
- `EfCoreBookRepositoryTests`：測試資料存取層 CRUD、查詢、異常、更新、刪除。
- `DatabaseFeatureTests`：資料庫遷移、SQL 查詢、欄位結構驗證。
- `ErrorHandlingTests`：錯誤處理、診斷資訊、健康檢查。
- `SimplifiedBookStoreTests`：展示 .NET Aspire Testing 基本概念與模型。
- `TestIsolationTests`：交易隔離、唯一資料測試，確保測試互不干擾。
- `TransactionTests`：交易回滾、並發一致性測試。
- `BookStoreDbTests`：預留資料庫測試擴充。

---

此範例專案展示如何使用 .NET Aspire 進行分散式應用程式的測試，包含 SQL Server 容器、EF Core、業務邏輯、資料存取層，以及多種測試隔離與交易處理技巧。測試專案涵蓋業務邏輯、資料庫遷移、錯誤處理、交易、隔離、並發等主題，提供完整的分散式應用測試最佳實踐。
