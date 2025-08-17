# Day 03：範例專案

[Day 03：xUnit 進階功能與測試資料管理](https://ithelp.ithome.com.tw/articles/10374064)

---

## 專案結構

```text
day03/
├── Day03.Samples.sln
├── src/
│   └── Day03.Core/
│       ├── Day03.Core.csproj
│       ├── GlobalUsing.cs
│       ├── bin/
│       ├── Data/
│       ├── Models/
│       ├── obj/
│       ├── Repositories/
│       └── Services/
└── tests/
    └── Day03.Core.Tests/
        ├── AdvancedTheoryTests/
        │   ├── CalculationTestData.cs
        │   ├── CalculatorAdvancedTests.cs
        │   ├── CalculatorClassDataTests.cs
        │   ├── ComplexObjectTests.cs
        │   ├── CsvDataTests.cs
        │   ├── CsvTestData.cs
        │   └── EmailValidatorTests.cs
        ├── BuilderPatternTests/
        │   ├── UserBuilder.cs
        │   ├── UserServiceTests.cs
        │   └── UserValidationTests.cs
        ├── FixtureTests/
        │   ├── Repositories/
        │   │   ├── DatabaseFixture.cs
        │   │   └── UserRepositoryTests.cs
        │   └── Services/
        │       ├── AnotherServiceTests.cs
        │       ├── ServiceCollectionDefinition.cs
        │       ├── ServiceFixture.cs
        │       └── ServiceIntegrationTests.cs
        ├── ParallelExecutionTests/
        │   ├── DefaultParallelTests.cs
        │   ├── SequentialTests.cs
        │   └── UserRepositoryTests.cs
        ├── TestData/
        ├── TestDataProviders/
        ├── Day03.Core.Tests.csproj
        ├── GlobalUsings.cs
        └── xunit.runner.json
```

---

## 專案內容簡介


### src/Day03.Core/
- **Day03.Core.csproj**：專案檔，定義核心程式的組建設定。
- **GlobalUsing.cs**：全域 using 設定。
- **Data/**、**Models/**、**Repositories/**、**Services/**：資料、模型、資料存取與服務層。

### tests/Day03.Core.Tests/

#### AdvancedTheoryTests/
- **CalculationTestData.cs**：提供計算相關的測試資料，供 [MemberData] 或 [ClassData] 屬性使用。
- **CalculatorAdvancedTests.cs**：進階計算機測試，展示 xUnit [Theory]、[MemberData] 的多型資料來源應用。
- **CalculatorClassDataTests.cs**：使用 [ClassData] 進行計算機測試，驗證不同資料來源的測試案例。
- **ComplexObjectTests.cs**：針對複雜物件結構進行測試，驗證多層級資料的正確性。
- **CsvDataTests.cs**：以 CSV 格式資料進行資料驅動測試，驗證資料解析與測試整合。
- **CsvTestData.cs**：提供 CSV 格式的測試資料來源實作。
- **EmailValidatorTests.cs**：針對電子郵件格式驗證進行單元測試。

#### BuilderPatternTests/
- **UserBuilder.cs**：實作 Builder 模式，用於建立測試用的 User 物件，提升測試資料的可讀性與彈性。
- **UserServiceTests.cs**：測試 UserService 服務，結合 Builder 模式產生測試資料，驗證服務邏輯。
- **UserValidationTests.cs**：針對 User 物件的驗證規則進行單元測試。

#### FixtureTests/
- **Repositories/DatabaseFixture.cs**：實作 IClassFixture，建立共用的資料庫測試環境，供多個測試類別重複使用。
- **Repositories/UserRepositoryTests.cs**：針對 UserRepository 進行單元測試，驗證資料存取邏輯，並利用 DatabaseFixture 共用資料庫狀態。
- **Services/AnotherServiceTests.cs**：測試另一個服務類別，驗證其在共用 ServiceFixture 下的行為。
- **Services/ServiceCollectionDefinition.cs**：定義 xUnit 測試 Collection，將多個測試類別歸為同一組，並關聯 ServiceFixture。
- **Services/ServiceFixture.cs**：建立 DI 服務提供者與 In-Memory Database，並初始化測試資料，供 Service 類測試共用。
- **Services/ServiceIntegrationTests.cs**：整合測試 Service 層，驗證多個服務在同一 DI/Fixture 下的互動。

#### ParallelExecutionTests/
- **DefaultParallelTests.cs**：展示 xUnit 預設的平行執行行為，驗證多個測試可同時執行。
- **SequentialTests.cs**：自訂 Collection，讓同一組測試依序執行，避免平行衝突。
- **UserRepositoryTests.cs**：驗證同一 Collection 下的測試不會平行執行，確保資料一致性。

#### TestData/
- （此目錄下檔案用於自訂測試資料屬性或資料來源，若有檔案會補充說明。）

#### TestDataProviders/
- **ITestDataProvider.cs**：定義泛型介面，提供有效、無效、邊界與範例測試資料的方法。
- **UserTestDataProvider.cs**：實作 ITestDataProvider，產生 User 物件的有效、無效、邊界與範例測試資料，供資料驅動測試使用。

- **Day03.Core.Tests.csproj**：測試專案檔，定義測試組建設定。
- **GlobalUsings.cs**：全域 using 設定。
- **xunit.runner.json**：xUnit 執行器設定檔。

---

此資料夾為 Day 03 的範例專案，重點在於 xUnit 進階功能（如 Theory、Fixture、平行/序列執行、Builder/資料驅動測試等）。
