# Day 04：範例專案

[Day 04：AwesomeAssertions 基礎應用與實戰技巧](https://ithelp.ithome.com.tw/articles/10374188)

---

## 專案結構

```text
day04/
├── Day04.Domain/
│   ├── Day04.Domain.csproj
│   ├── bin/
│   ├── Models/
│   │   ├── Address.cs
│   │   ├── ApiResponse.cs
│   │   ├── CreateUserRequest.cs
│   │   ├── IUser.cs
│   │   ├── Order.cs
│   │   ├── OrderItem.cs
│   │   ├── User.cs
│   │   └── UserProfile.cs
│   ├── obj/
│   └── Services/
│       ├── ApiService.cs
│       ├── Calculator.cs
│       ├── OrderService.cs
│       ├── UserService.cs
│       └── ValidationService.cs
├── Day04.Domain.Tests/
│   ├── BasicAssertions/
│   │   ├── AsyncAssertionTests.cs
│   │   ├── CollectionAssertionTests.cs
│   │   ├── ComplexObjectComparisonTests.cs
│   │   ├── ExceptionAssertionTests.cs
│   │   ├── NumericAssertionTests.cs
│   │   ├── ObjectAssertionTests.cs
│   │   ├── ObjectComparisonTests.cs
│   │   └── StringAssertionTests.cs
│   ├── PracticalExamples/
│   │   ├── AssertionStyleComparison.cs
│   │   ├── DomainSpecificAssertionPatterns.cs
│   │   ├── ReadableAssertionTests.cs
│   │   └── UserServiceTests.cs
│   ├── bin/
│   ├── Day04.Domain.Tests.csproj
│   ├── GlobalUsings.cs
│   └── obj/
└── Day04.Samples.sln
```

---

## 專案內容簡介

### Day04.Domain/
- **Models/**：定義各種領域模型（如 User、Order、Address 等）。
- **Services/**：提供業務邏輯服務（如 Calculator、UserService、OrderService 等）。

### Day04.Domain.Tests/

**BasicAssertions/**：
  - `AsyncAssertionTests.cs`：非同步斷言測試，驗證 async/await 任務的完成狀態與例外處理。
  - `CollectionAssertionTests.cs`：集合斷言測試，檢查集合的數量、內容、排序、唯一性與複雜物件集合條件。
  - `ComplexObjectComparisonTests.cs`：複雜物件比對，驗證巢狀屬性、集合與物件等價性，支援完整與部分屬性比較。
  - `ExceptionAssertionTests.cs`：例外斷言測試，驗證例外型別、訊息、參數與特定例外類型。
  - `NumericAssertionTests.cs`：數值斷言測試，檢查範圍、精度、特殊值與運算結果。
  - `ObjectAssertionTests.cs`：物件斷言測試，驗證型別、屬性、等價性、Null 狀態與屬性值。
  - `ObjectComparisonTests.cs`：物件比對測試，針對深層屬性、Profile 等複合物件進行等價性與排除特定屬性驗證。
  - `StringAssertionTests.cs`：字串斷言測試，檢查開頭、結尾、正則、等價、格式與忽略大小寫。

**PracticalExamples/**：
  - `AssertionStyleComparison.cs`：斷言風格比較，對比傳統 Assert 與 AwesomeAssertions 的可讀性、表達力與錯誤訊息。
  - `DomainSpecificAssertionPatterns.cs`：領域專屬斷言模式，展示業務規則、API 回應、AssertionScope 多重驗證等技巧。
  - `ReadableAssertionTests.cs`：可讀性斷言測試，強調命名、斷言結構、明確期望值與錯誤訊息。
  - `UserServiceTests.cs`：使用者服務測試，涵蓋多情境、命名規範、無效資料例外、啟用狀態等多元驗證。

---

此資料夾為 Day 04 的範例專案，重點在於 AwesomeAssertions Library 的基礎用法與實戰技巧。
