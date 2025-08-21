# Day 12 – 範例專案

> [Day 12 – 結合 AutoData：xUnit 與 AutoFixture 的整合應用](https://ithelp.ithome.com.tw/articles/10375296)

## 專案結構

```
day12/
├── Day12.Samples.sln
├── src/
│   └── Day12.Core/
│       ├── Day12.Core.csproj
│       ├── Enums/
│       │   └── OrderStatus.cs
│       └── Models/
│           ├── CategorizedProduct.cs
│           ├── Customer.cs
│           ├── CustomerJsonRecord.cs
│           ├── Order.cs
│           ├── OrderItem.cs
│           ├── OrderResult.cs
│           ├── Person.cs
│           ├── Product.cs
│           └── ProductCsvRecord.cs
└── tests/
    └── Day12.Core.Tests/
        ├── Day12.Core.Tests.csproj
        ├── GlobalUsings.cs
        ├── AutoDataBasicTests.cs
        ├── AwesomeAssertionsCollaborationTests.cs
        ├── CollectionSizeTests.cs
        ├── CompositeAutoDataTests.cs
        ├── DataSourceDesignPatternTests.cs
        ├── ExternalDataIntegrationTests.cs
        ├── InlineAutoDataTests.cs
        ├── MemberAutoDataTests.cs
        ├── Attributes/
        │   ├── BusinessAutoDataAttribute.cs
        │   ├── CollectionSizeAttribute.cs
        │   ├── CompositeAutoDataAttribute.cs
        │   └── DomainAutoDataAttribute.cs
        ├── DataSources/
        │   ├── BaseTestData.cs
        │   ├── CustomerTestDataSource.cs
        │   ├── ProductTestDataSource.cs
        │   └── ReusableTestDataSets.cs
        └── TestData/
            ├── customers.json
            └── products.csv
```

---

## 專案內容說明

### 方案與專案

- **Day12.Samples.sln**  
  Visual Studio 方案檔，整合 src 與 tests 兩個專案。

#### src/Day12.Core

- **Day12.Core.csproj**  
  主程式庫專案檔，目標 .NET 9。
- **Enums/OrderStatus.cs**  
  訂單狀態列舉，定義 Created、Confirmed、Shipped、Delivered 等狀態。
- **Models/CategorizedProduct.cs**  
  分類產品資料，含產品與分類資訊。
- **Models/Customer.cs**  
  客戶資料，包含個人資訊、類型、信用額度等。
- **Models/CustomerJsonRecord.cs**  
  對應 JSON 測試資料的客戶記錄類別。
- **Models/Order.cs**  
  訂單資料，含狀態、編號、客戶、明細等。
- **Models/OrderItem.cs**  
  訂單明細，含產品、數量等。
- **Models/OrderResult.cs**  
  訂單處理結果模型。
- **Models/Person.cs**  
  人員基本資料，含 Guid、Name、Age 等，支援 DataAnnotations。
- **Models/Product.cs**  
  產品資料，含名稱、價格、是否可販售等。
- **Models/ProductCsvRecord.cs**  
  對應 CSV 測試資料的產品記錄類別。

#### tests/Day12.Core.Tests

- **Day12.Core.Tests.csproj**  
  測試專案檔，參考 AutoFixture.Xunit2、xUnit、AwesomeAssertions、CsvHelper 等套件。
- **GlobalUsings.cs**  
  測試專案全域 using 設定。
- **AutoDataBasicTests.cs**  
  AutoData 基本用法測試，驗證自動產生參數與 DataAnnotations 整合。
- **AwesomeAssertionsCollaborationTests.cs**  
  AutoData 搭配 AwesomeAssertions 進行多型態驗證，並結合外部資料來源。
- **CollectionSizeTests.cs**  
  測試自訂 CollectionSizeAttribute 控制集合產生數量。
- **CompositeAutoDataTests.cs**  
  測試自訂 CompositeAutoDataAttribute，整合多個 AutoData 配置。
- **DataSourceDesignPatternTests.cs**  
  示範資料來源設計模式，集中管理可重用測試資料。
- **ExternalDataIntegrationTests.cs**  
  測試如何整合外部 JSON/CSV 檔案作為測試資料來源。
- **InlineAutoDataTests.cs**  
  測試 InlineAutoData 混合固定值與自動產生資料。
- **MemberAutoDataTests.cs**  
  測試 MemberAutoDataAttribute，結合靜態資料來源與 AutoFixture。

##### Attributes/ (自訂 Attribute)

- **BusinessAutoDataAttribute.cs**  
  針對業務邏輯自訂的 AutoData，預設產生特定狀態與金額的訂單。
- **CollectionSizeAttribute.cs**  
  控制集合參數產生數量的自訂 Attribute。
- **CompositeAutoDataAttribute.cs**  
  支援多個 AutoData 組合的自訂 Attribute。
- **DomainAutoDataAttribute.cs**  
  針對領域物件自訂的 AutoData，產生特定格式的人員資料。

##### DataSources/ (測試資料來源)

- **BaseTestData.cs**  
  測試資料來源基底類別，提供路徑工具。
- **CustomerTestDataSource.cs**  
  客戶測試資料來源，讀取 JSON 檔案產生資料。
- **ProductTestDataSource.cs**  
  產品測試資料來源，支援基本資料與 CSV 檔案讀取。
- **ReusableTestDataSets.cs**  
  可重用的產品分類資料集。

##### TestData/ (外部測試資料)

- **customers.json**  
  客戶測試資料，供資料來源類別讀取。
- **products.csv**  
  產品測試資料，供資料來源類別讀取。

---

## 測試類別重點說明

- **AutoDataBasicTests**  
  驗證 AutoData 能自動產生所有參數，並支援 DataAnnotations。
- **AwesomeAssertionsCollaborationTests**  
  結合 AutoData 與 AwesomeAssertions，並整合外部資料來源。
- **CollectionSizeTests**  
  測試自訂 Attribute 控制集合產生數量。
- **CompositeAutoDataTests**  
  測試多個 AutoData 組合，提升測試彈性。
- **DataSourceDesignPatternTests**  
  示範集中管理測試資料來源的設計模式。
- **ExternalDataIntegrationTests**  
  測試如何將 JSON/CSV 外部檔案整合進測試流程。
- **InlineAutoDataTests**  
  測試混合固定值與自動產生資料的情境。
- **MemberAutoDataTests**  
  測試結合靜態資料來源與 AutoFixture 的進階用法。

---

本專案示範如何在 xUnit 測試中，結合 AutoFixture 的 AutoData、InlineAutoData、MemberAutoData 等進階用法，並搭配自訂 Attribute 與外部資料來源，讓測試資料產生更彈性、可維護。
