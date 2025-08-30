# Day 21 – 範例專案

> [Day 21 – Testcontainers 整合測試：MSSQL + EF Core 以及 Dapper 基礎應用](https://ithelp.ithome.com.tw/articles/10376524)

---

## 專案結構

```text
day21/
├── Day21.Samples.sln                        # 方案檔
├── .gitignore                               # 忽略設定
├── src/
│   └── Day21.Core/
│       ├── Day21.Core.csproj                # 核心專案 csproj
│       ├── GlobalUsings.cs                  # 全域 using 設定
│       ├── Data/
│       │   └── ECommerceDbContext.cs        # 電子商務資料庫 DbContext
│       ├── Models/
│       │   ├── Category.cs                  # 商品分類模型
│       │   ├── Customer.cs                  # 客戶模型
│       │   ├── Order.cs                     # 訂單模型
│       │   ├── OrderItem.cs                 # 訂單項目模型
│       │   ├── Product.cs                   # 商品模型
│       │   ├── ProductTag.cs                # 商品標籤關聯模型
│       │   └── Tag.cs                       # 標籤模型
│       └── Repositories/
│           ├── DapperProductRepository.cs   # Dapper 產品資料存取實作
│           ├── EfCoreProductRepository.cs   # EF Core 產品資料存取實作
│           ├── IProductByDapperRepository.cs# Dapper 進階查詢介面與報表模型
│           ├── IProductByEFCoreRepository.cs# EF Core 進階查詢介面
│           └── IProductRepository.cs        # 產品資料存取共用介面
├── tests/
│   └── Day21.Core.Tests/
│       ├── Day21.Core.Tests.csproj          # 測試專案 csproj
│       ├── GlobalUsings.cs                  # 測試專案全域 using 設定
│       ├── Infrastructure/
│       │   └── SqlServerContainerFixture.cs # Testcontainers SQL Server 容器管理
│       ├── EfCoreCrudTests.cs               # EF Core CRUD 測試
│       ├── EfCoreAdvancedTests.cs           # EF Core 進階功能測試
│       ├── DapperCrudTests.cs               # Dapper CRUD 測試
│       ├── DapperAdvancedTests.cs           # Dapper 進階功能測試
│       └── SqlScripts/
│           ├── Tables/
│           │   ├── CreateCategoriesTable.sql        # 建立 Categories 表
│           │   ├── CreateCustomersTable.sql         # 建立 Customers 表
│           │   ├── CreateOrderItemsTable.sql        # 建立 OrderItems 表
│           │   ├── CreateOrdersTable.sql            # 建立 Orders 表
│           │   ├── CreateProductTagsTable.sql       # 建立 ProductTags 關聯表
│           │   ├── CreateProductsTable.sql          # 建立 Products 表
│           │   └── CreateTagsTable.sql              # 建立 Tags 表
│           └── StoredProcedures/
│               └── CreateProductSalesReportStoredProcedure.sql # 建立產品銷售報表預存程序
```

---

## 專案內容簡介

### 1. `src/Day21.Core/`
- **Day21.Core.csproj**：專案描述與套件依賴，支援 EF Core、Dapper、MSSQL。
- **GlobalUsings.cs**：全域 using 設定，簡化程式碼。
- **Data/ECommerceDbContext.cs**：電子商務資料庫 DbContext，定義所有資料表。
- **Models/**：資料庫實體模型
  - **Category.cs**：商品分類資料模型。
  - **Customer.cs**：客戶資料模型。
  - **Order.cs**：訂單資料模型。
  - **OrderItem.cs**：訂單項目資料模型。
  - **Product.cs**：商品資料模型。
  - **ProductTag.cs**：商品與標籤多對多關聯模型。
  - **Tag.cs**：標籤資料模型。
- **Repositories/**
  - **IProductRepository.cs**：產品資料存取共用介面，CRUD 操作。
  - **EfCoreProductRepository.cs**：以 EF Core 實作的產品資料存取。
  - **DapperProductRepository.cs**：以 Dapper 實作的產品資料存取。
  - **IProductByEFCoreRepository.cs**：EF Core 進階查詢介面（如 Include、SplitQuery、批次更新/刪除等）。
  - **IProductByDapperRepository.cs**：Dapper 進階查詢介面（如 QueryMultiple、動態查詢、預存程序等），內含 ProductSalesReport 報表模型。

### 2. `tests/Day21.Core.Tests/`
- **Day21.Core.Tests.csproj**：測試專案描述，引用 Testcontainers、xUnit、Dapper、EF Core 等。
- **GlobalUsings.cs**：測試專案全域 using 設定。
- **Infrastructure/SqlServerContainerFixture.cs**：Testcontainers 管理 SQL Server 容器生命週期，提供測試用資料庫環境。
- **EfCoreCrudTests.cs**：EF Core CRUD 操作測試，驗證基本增刪查改功能。
- **EfCoreAdvancedTests.cs**：EF Core 進階功能測試，包含 Include 關聯查詢、SplitQuery、批次更新/刪除、唯讀查詢等。
- **DapperCrudTests.cs**：Dapper CRUD 操作測試，驗證 Dapper 操作資料的正確性。
- **DapperAdvancedTests.cs**：Dapper 進階功能測試，包含 QueryMultiple、DynamicParameters、預存程序查詢等。
- **SqlScripts/Tables/**：各資料表建立 SQL 腳本，供測試初始化用。
  - **CreateCategoriesTable.sql**：建立商品分類表。
  - **CreateCustomersTable.sql**：建立客戶表。
  - **CreateOrderItemsTable.sql**：建立訂單項目表。
  - **CreateOrdersTable.sql**：建立訂單表。
  - **CreateProductTagsTable.sql**：建立商品標籤關聯表。
  - **CreateProductsTable.sql**：建立商品表。
  - **CreateTagsTable.sql**：建立標籤表。
- **SqlScripts/StoredProcedures/**
  - **CreateProductSalesReportStoredProcedure.sql**：建立產品銷售報表預存程序，供 Dapper 進階查詢測試。

### 3. 其他
- **Day21.Samples.sln**：Visual Studio 方案檔。
- **.gitignore**：忽略 .idea 等開發相關目錄。

---

## 測試專案各測試類別說明

- **EfCoreCrudTests.cs**：針對 EF Core Repository 進行基本 CRUD 測試，確保資料正確新增、查詢、更新、刪除。
- **EfCoreAdvancedTests.cs**：測試 EF Core 進階查詢（如 Include 載入關聯、SplitQuery、批次更新/刪除、AsNoTracking 唯讀查詢等）。
- **DapperCrudTests.cs**：針對 Dapper Repository 進行基本 CRUD 測試，驗證 Dapper 操作資料的正確性。
- **DapperAdvancedTests.cs**：測試 Dapper 進階查詢（如 QueryMultiple 載入關聯、DynamicParameters 動態查詢、預存程序查詢與報表產生等）。
- **Infrastructure/SqlServerContainerFixture.cs**：負責啟動/關閉 SQL Server Docker 容器，並提供測試用連線字串。

---

此專案展示如何以 Testcontainers 建立 MSSQL 測試環境，並結合 EF Core 與 Dapper 進行整合測試與進階查詢。
