# Day 05：AwesomeAssertions 進階技巧與複雜情境應用

[原文連結](https://ithelp.ithome.com.tw/articles/10374425)

---

## 專案結構

```text
day05/
├── src/
│   └── Day05.Domain/
│       ├── Day05.Domain.csproj
│       ├── bin/
│       ├── DomainModels/
│       │   ├── AuditInfo.cs
│       │   ├── ComplexObject.cs
│       │   ├── DataRecord.cs
│       │   ├── Order.cs
│       │   ├── OrderItem.cs
│       │   ├── PaymentRequest.cs
│       │   ├── PaymentResult.cs
│       │   ├── Product.cs
│       │   ├── TreeNode.cs
│       │   ├── User.cs
│       │   ├── UserEntity.cs
│       │   └── UserProfile.cs
│       ├── Exceptions/
│       ├── Models/
│       ├── obj/
│       └── Services/
│           ├── BusinessServices/
│           ├── ProcessingServices/
│           └── UserServices/
├── tests/
│   └── Day05.Domain.Tests/
│       ├── AdvancedAsyncAssertionTests/
│       │   └── AdvancedAsyncAssertionTests.cs
│       ├── AdvancedExceptionAssertionTests/
│       │   └── AdvancedExceptionAssertionTests.cs
│       ├── AdvancedObjectGraphTests/
│       │   └── AdvancedObjectGraphTests.cs
│       ├── CustomAssertionTests/
│       │   ├── ConditionalAssertionTests.cs
│       │   ├── OrderServiceTests.cs
│       │   └── ProductServiceTests.cs
│       ├── DynamicFieldExclusionTests/
│       │   └── DynamicFieldExclusionTests.cs
│       ├── Extensions/
│       ├── PerformanceOptimizedTests/
│       │   ├── ErrorMessageOptimizationTests.cs
│       │   └── PerformanceOptimizedTests.cs
│       ├── bin/
│       ├── Day05.Domain.Tests.csproj
│       ├── GlobalUsings.cs
│       └── obj/
└── Day05.Samples.sln
```

---

## 專案內容簡介

### src/Day05.Domain/
- **DomainModels/**：複雜領域模型（如 User、Order、Product、TreeNode 等）與資料結構。
- **Services/**：分層業務邏輯服務。

### tests/Day05.Domain.Tests/

- **AdvancedAsyncAssertionTests/**：
  - `AdvancedAsyncAssertionTests.cs`：進階非同步斷言技巧，驗證 async/await 行為與例外。

- **AdvancedExceptionAssertionTests/**：
  - `AdvancedExceptionAssertionTests.cs`：進階例外驗證，包含巢狀例外、動態訊息等。

- **AdvancedObjectGraphTests/**：
  - `AdvancedObjectGraphTests.cs`：複雜物件圖與巢狀結構的等價性與差異比對。

- **CustomAssertionTests/**：
  - `ConditionalAssertionTests.cs`：條件式斷言與自訂驗證情境。
  - `OrderServiceTests.cs`、`ProductServiceTests.cs`：服務層自訂斷言與業務驗證。

- **DynamicFieldExclusionTests/**：
  - `DynamicFieldExclusionTests.cs`：動態排除欄位、部分比對等進階應用。
  
- **PerformanceOptimizedTests/**：
  - `PerformanceOptimizedTests.cs`：針對大數據量與高效能場景設計的斷言測試，包含大集合抽樣驗證、效能敏感比對等最佳化技巧。
  - `ErrorMessageOptimizationTests.cs`：專注於錯誤訊息品質與除錯體驗，驗證失敗時能提供具體、明確的錯誤上下文與程式碼。

- **Extensions/**：
  - `CustomAssertions.cs`：專案自訂斷言擴充，針對電商領域（如 Product、Order）提供語意化驗證方法，提升測試可讀性與重用性。
  - `PerformanceAssertions.cs`：大量資料比對與效能優化斷言，支援分批比對、關鍵屬性快速驗證等高效測試技巧。

---

此資料夾為 Day 05 的範例專案，重點在於 AwesomeAssertions 進階技巧、複雜情境與自訂驗證。
