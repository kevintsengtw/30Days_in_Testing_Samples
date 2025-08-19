# Day 10：範例專案

> [Day 10：AutoFixture 基礎：自動產生測試資料](https://ithelp.ithome.com.tw/articles/10375018)

---

## 專案結構

```
day10/
├── Day10.Samples.sln
├── src/
│   └── Day10.Core/
│       ├── Day10.Core.csproj
│       ├── GlobalUsings.cs
│       ├── Dtos/
│       │   ├── CreateCustomerRequest.cs
│       │   ├── ProductUpdateRequest.cs
│       │   └── RequestDtos.cs
│       ├── Enums/
│       │   ├── CustomerLevel.cs
│       │   ├── CustomerType.cs
│       │   └── OrderStatus.cs
│       ├── Models/
│       │   ├── Address.cs
│       │   ├── Category.cs
│       │   ├── ContactInfo.cs
│       │   ├── Customer.cs
│       │   ├── CustomerType.cs
│       │   ├── GeoLocation.cs
│       │   ├── Order.cs
│       │   ├── OrderItem.cs
│       │   └── Product.cs
│       ├── Services/
│       │   ├── BatchOrderProcessor.cs
│       │   ├── BatchProcessResult.cs
│       │   ├── DataProcessor.cs
│       │   ├── DataRecord.cs
│       │   ├── DiscountCalculator.cs
│       │   ├── EmailService.cs
│       │   ├── MemberLevel.cs
│       │   ├── OrderCalculator.cs
│       │   ├── OrderProcessor.cs
│       │   ├── PriceCalculator.cs
│       │   ├── ProcessingResult.cs
│       │   ├── ProcessResult.cs
│       │   ├── ProductService.cs
│       │   ├── User.cs
│       │   └── UserService.cs
│       └── Validators/
│           ├── CustomerValidator.cs
│           ├── ProductValidator.cs
│           └── UserValidator.cs
└── tests/
    └── Day10.Core.Tests/
        ├── AutoDataWithCircularReferenceHandling.cs
        ├── AutoFixtureTestBase.cs
        ├── BasicGeneration/
        │   ├── BasicTypesGenerationTests.cs
        │   └── OmitAutoPropertiesTests.cs
        ├── AdvancedPreview/
        │   └── AdvancedTechniquesPreviewTests.cs
        ├── Comparison/
        │   ├── Day03TraditionalApproachTests.cs
        │   ├── Day10AutoFixtureApproachTests.cs
        │   └── PerformanceComparisonTests.cs
        ├── ComplexObjects/
        │   └── ComplexObjectCreationTests.cs
        ├── PracticalScenarios/
        │   ├── DtoValidationTests.cs
        │   ├── EntityTests.cs
        │   └── LargeDataScenarioTests.cs
        ├── StabilityAndPredictability/
        │   └── StabilityTests.cs
        ├── XunitIntegration/
        │   ├── SharedFixtureTests.cs
        │   └── XunitIntegrationTests.cs
        └── GlobalUsings.cs
```

---

## 專案內容簡介

### src/Day10.Core/
- **Day10.Core.csproj**：專案檔，.NET 9 設定。
- **GlobalUsings.cs**：全域 using 設定。

#### Dtos/
- **CreateCustomerRequest.cs**：建立客戶請求 DTO，含姓名、Email 等欄位。
- **ProductUpdateRequest.cs**：產品更新請求 DTO。
- **RequestDtos.cs**：產品建立請求 DTO。

#### Enums/
- **CustomerLevel.cs**：客戶等級（銅、銀、金、鑽石）。
- **CustomerType.cs**：客戶類型（一般、高級、VIP）。
- **OrderStatus.cs**：訂單狀態（待處理、處理中、已完成等）。

#### Models/
- **Address.cs**：地址資訊。
- **Category.cs**：產品分類，支援巢狀結構。
- **ContactInfo.cs**：聯絡資訊。
- **Customer.cs**：客戶實體，含基本資料、聯絡方式、等級等。
- **CustomerType.cs**：空檔案（保留，未使用）。
- **GeoLocation.cs**：地理位置。
- **Order.cs**：訂單主體。
- **OrderItem.cs**：訂單項目。
- **Product.cs**：產品主體。

#### Services/
- **BatchOrderProcessor.cs**：批次訂單處理器。
- **BatchProcessResult.cs**：批次處理結果。
- **DataProcessor.cs**：資料處理器，批次處理資料。
- **DataRecord.cs**：資料記錄。
- **DiscountCalculator.cs**：折扣計算器。
- **EmailService.cs**：電子郵件服務。
- **MemberLevel.cs**：會員等級（enum）。
- **OrderCalculator.cs**：訂單金額計算。
- **OrderProcessor.cs**：訂單處理器。
- **PriceCalculator.cs**：價格計算器。
- **ProcessingResult.cs**：資料處理結果。
- **ProcessResult.cs**：單一處理結果。
- **ProductService.cs**：產品服務，含建立產品等。
- **User.cs**：使用者主體。
- **UserService.cs**：使用者服務，含建立使用者等。

#### Validators/
- **CustomerValidator.cs**：客戶驗證器，檢查是否成年。
- **ProductValidator.cs**：產品驗證器，檢查物件是否有效。
- **UserValidator.cs**：使用者驗證器，檢查使用者資料有效性。

### tests/Day10.Core.Tests/
- **AutoDataWithCircularReferenceHandling.cs**：AutoFixture 處理循環參考的測試。
- **AutoFixtureTestBase.cs**：AutoFixture 測試基底類別。
- **GlobalUsings.cs**：全域 using 設定。

#### BasicGeneration/
- **BasicTypesGenerationTests.cs**：AutoFixture 產生基本型別（字串、數字等）測試。
- **OmitAutoPropertiesTests.cs**：OmitAutoProperties 用法測試。

#### AdvancedPreview/
- **AdvancedTechniquesPreviewTests.cs**：AutoFixture 進階技術預覽測試。

#### Comparison/
- **Day03TraditionalApproachTests.cs**：Day03 傳統手動產生資料測試。
- **Day10AutoFixtureApproachTests.cs**：Day10 使用 AutoFixture 產生資料測試。
- **PerformanceComparisonTests.cs**：傳統與 AutoFixture 效能比較測試。

#### ComplexObjects/
- **ComplexObjectCreationTests.cs**：AutoFixture 建立複雜物件測試。

#### PracticalScenarios/
- **DtoValidationTests.cs**：DTO 驗證測試。
- **EntityTests.cs**：Entity 層級測試，驗證客戶等級等。
- **LargeDataScenarioTests.cs**：大量資料處理測試。

#### StabilityAndPredictability/
- **StabilityTests.cs**：穩定性與可預測性測試。

#### XunitIntegration/
- **SharedFixtureTests.cs**：xUnit 共用 Fixture 測試。
- **XunitIntegrationTests.cs**：AutoFixture 與 xUnit 整合測試。

---

本資料夾為 Day 10 範例專案，重點在於 AutoFixture 的自動產生測試資料技巧，涵蓋 DTO、模型、服務、驗證器與多種實戰測試案例。
