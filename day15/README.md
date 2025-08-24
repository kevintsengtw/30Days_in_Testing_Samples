# Day 15 – 範例專案

> [Day 15 – AutoFixture 與 Bogus 的整合應用](https://ithelp.ithome.com.tw/articles/10375620)

---

## 專案結構

```
day15/
├── Day15.Samples.sln
├── src/
│   └── Day15.Core/
│       ├── Day15.Core.csproj
│       └── Models/
│           ├── Address.cs
│           ├── Company.cs
│           ├── Order.cs
│           ├── OrderItem.cs
│           ├── OrderStatus.cs
│           ├── Product.cs
│           └── User.cs
└── tests/
    ├── Day15.Core.Tests/
    │   ├── Day15.Core.Tests.csproj
    │   ├── BogusAutoDataAttribute.cs
    │   ├── IntegratedTestDataTests.cs
    │   ├── PerformanceTests.cs
    │   ├── RealWorldApplicationTests.cs
    │   └── SeedManagementTests.cs
    └── Day15.TestLibrary/
        ├── Day15.TestLibrary.csproj
        └── TestData/
            ├── Base/
            │   └── TestBase.cs
            ├── Extensions/
            │   └── FixtureExtensions.cs
            ├── Factories/
            │   └── IntegratedTestDataFactory.cs
            ├── SpecimenBuilders/
            │   ├── AddressSpecimenBuilder.cs
            │   ├── BogusSpecimenBuilder.cs
            │   ├── CompanyNameSpecimenBuilder.cs
            │   ├── EmailSpecimenBuilder.cs
            │   ├── NameSpecimenBuilder.cs
            │   ├── PhoneSpecimenBuilder.cs
            │   ├── SeedAwareBogusSpecimenBuilder.cs
            │   └── WebsiteSpecimenBuilder.cs
            ├── HybridTestDataGenerator.cs
            └── ITestDataGenerator.cs
```

---

## 專案內容說明

### src/Day15.Core

#### Models/
- **Address.cs**：地址資訊實體，包含街道、城市、郵遞區號等屬性。
- **Company.cs**：公司資訊實體，包含公司名稱、行業、網站、電話等屬性。
- **Order.cs**：訂單實體，包含訂單日期、總金額、客戶及訂單項目列表。
- **OrderItem.cs**：訂單項目實體，包含產品、數量、單價等屬性。
- **OrderStatus.cs**：訂單狀態列舉，定義待處理、已確認、已出貨等狀態。
- **Product.cs**：產品實體，包含產品名稱、描述、價格、類別等屬性。
- **User.cs**：使用者實體，包含姓名、電子郵件、電話、地址、公司等完整屬性。

### tests/Day15.Core.Tests

- **BogusAutoDataAttribute.cs**：自訂 AutoData 屬性，整合 Bogus 功能到 xUnit 參數化測試中。
- **IntegratedTestDataTests.cs**：整合測試資料測試，驗證 AutoFixture 與 Bogus 整合後的資料品質與正確性。
- **PerformanceTests.cs**：效能測試，測量大量資料產生的執行時間與記憶體使用情況。
- **RealWorldApplicationTests.cs**：實際應用場景測試，模擬真實業務邏輯中的資料產生需求。
- **SeedManagementTests.cs**：種子管理測試，驗證相同種子產生可重現的測試資料。

### tests/Day15.TestLibrary

#### TestData/Base/
- **TestBase.cs**：測試基底類別，提供統一的資料產生功能，包含 AutoFixture、混合產生器及整合工廠。

#### TestData/Extensions/
- **FixtureExtensions.cs**：AutoFixture 擴充方法，設定 Bogus 整合、循環參考處理及各種 SpecimenBuilder。

#### TestData/Factories/
- **IntegratedTestDataFactory.cs**：整合測試資料工廠，提供高階物件建立功能，支援快取與種子設定。

#### TestData/SpecimenBuilders/
- **AddressSpecimenBuilder.cs**：地址資料專用 SpecimenBuilder，產生真實感的地址資訊。
- **BogusSpecimenBuilder.cs**：核心 Bogus 整合 SpecimenBuilder，整合各種 Faker 到 AutoFixture 中。
- **CompanyNameSpecimenBuilder.cs**：公司名稱專用 SpecimenBuilder，產生真實的公司名稱。
- **EmailSpecimenBuilder.cs**：電子郵件專用 SpecimenBuilder，產生有效格式的電子郵件地址。
- **NameSpecimenBuilder.cs**：姓名專用 SpecimenBuilder，產生真實的人名。
- **PhoneSpecimenBuilder.cs**：電話號碼專用 SpecimenBuilder，產生有效格式的電話號碼。
- **SeedAwareBogusSpecimenBuilder.cs**：支援種子設定的 Bogus SpecimenBuilder，確保資料可重現性。
- **WebsiteSpecimenBuilder.cs**：網站網址專用 SpecimenBuilder，產生有效的網站網址。

#### TestData/
- **HybridTestDataGenerator.cs**：混合資料產生器實作，結合 AutoFixture 與 Bogus 的優勢，支援種子設定與可重現性。
- **ITestDataGenerator.cs**：統一的測試資料產生介面，定義標準的資料產生方法。

---

## 整合特色

- **雙重優勢**：結合 AutoFixture 的物件自動建構能力與 Bogus 的真實資料產生能力。
- **靈活配置**：透過 SpecimenBuilder 精確控制每個屬性的資料產生邏輯。
- **可重現性**：支援種子設定，確保測試結果可重現。
- **效能最佳化**：透過快取機制減少重複計算，提升大量資料產生效能。
- **實用工具**：提供測試基底類別與工廠模式，簡化測試程式碼撰寫。

---

本專案示範如何將 `AutoFixture` 與 `Bogus` 進行深度整合，結合兩者優勢來產生高品質的測試資料。內容包含：

- AutoFixture 與 Bogus 的整合策略
- 自訂 SpecimenBuilder 整合 Bogus 資料產生
- 混合資料產生器設計模式
- 可重現性種子管理
- 效能最佳化與實際應用場景