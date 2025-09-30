# Day 18 – 範例專案

> [Day 18 – 驗證測試：FluentValidation Test Extensions](https://ithelp.ithome.com.tw/articles/10376147)

---

## 專案結構

```text
day18/
├── Day18.Samples.sln
├── src/
│   └── Day18.Core/
│       ├── Day18.Core.csproj
│       ├── Models/
│       │   └── UserRegistrationRequest.cs
│       ├── Services/
│       │   └── IUserService.cs
│       └── Validators/
│           ├── UserRegistrationAsyncValidator.cs
│           └── UserRegistrationValidator.cs
└── tests/
    └── Day18.Core.Tests/
        ├── Day18.Core.Tests.csproj
        ├── Extensions/
        │   └── FakeTimeProviderExtensions.cs
        ├── GlobalUsings.cs
        └── Validators/
            ├── ConditionalValidationTests.cs
            ├── FakeTimeProviderExtensionDemoTests.cs
            ├── RoleValidationTests.cs
            ├── UserRegistrationAsyncValidatorTests.cs
            └── UserRegistrationValidatorTests.cs
```

---

## 專案內容說明

### src/Day18.Core

- **Models/UserRegistrationRequest.cs**：使用者註冊請求資料模型，包含使用者名稱、信箱、密碼、確認密碼、生日、年齡、電話號碼、角色清單、使用條款同意等完整欄位。
- **Services/IUserService.cs**：用戶服務介面，提供帳號和信箱的可用性查詢功能，供非同步驗證器使用。
- **Validators/UserRegistrationValidator.cs**：同步驗證器，整合 TimeProvider 進行時間相關驗證，包含使用者名稱、信箱格式、密碼複雜度、年齡與生日一致性、條件式電話號碼驗證、角色有效性等規則。
- **Validators/UserRegistrationAsyncValidator.cs**：非同步驗證器，結合 IUserService 進行帳號唯一性和信箱重複性的非同步驗證，同時包含所有同步驗證規則。
- **Day18.Core.csproj**：主專案組態檔。

### tests/Day18.Core.Tests

- **Validators/UserRegistrationValidatorTests.cs**：同步驗證器的完整單元測試，涵蓋各欄位驗證規則、錯誤訊息、時間相關驗證等場景。
- **Validators/UserRegistrationAsyncValidatorTests.cs**：非同步驗證器的單元測試，驗證帳號唯一性、信箱重複性檢查等非同步驗證流程。
- **Validators/ConditionalValidationTests.cs**：條件式驗證專門測試，重點驗證電話號碼等可選欄位在有值時才進行驗證的情境。
- **Validators/RoleValidationTests.cs**：角色欄位驗證專門測試，驗證角色清單的有效性、空值處理等場景。
- **Validators/FakeTimeProviderExtensionDemoTests.cs**：展示 FakeTimeProvider 擴充方法使用的示範測試，說明如何簡化時間設定操作。
- **Extensions/FakeTimeProviderExtensions.cs**：FakeTimeProvider 的輔助擴充方法，提供 SetLocalNow 方法簡化本地時間設定。
- **GlobalUsings.cs**：測試專案全域 using 設定，包含 Xunit、FluentValidation、AwesomeAssertions、FakeTimeProvider 等相關命名空間。
- **Day18.Core.Tests.csproj**：測試專案組態檔。

---

## 技術重點

- **FluentValidation 驗證器**：同步/非同步驗證規則設計，支援複雜的多欄位、條件式、角色、時間相關等驗證情境。
- **Test Extensions 斷言**：使用 FluentValidation.TestHelper 進行精確的欄位錯誤、訊息、條件驗證等斷言，提升測試可讀性和維護性。
- **TimeProvider 整合**：透過 Microsoft.Extensions.Time.Testing 的 FakeTimeProvider 實現可控時間驗證，確保時間相關規則具有可重現性。
- **FakeTimeProvider 擴充方法**：自訂 SetLocalNow 擴充方法，簡化本地時間設定操作，提供更直觀的測試時間配置方式。
- **依賴注入與模擬**：非同步驗證器透過 IUserService 注入外部服務，使用 NSubstitute 進行模擬，實現獨立的單元測試。
- **條件式驗證測試**：專門針對 When 條件的驗證邏輯進行測試，確保可選欄位的驗證行為正確。

---

本專案示範如何在 .NET 專案中使用 [FluentValidation](https://fluentvalidation.net/) 及其 Test Extensions 進行驗證邏輯的單元測試。內容包含：

- FluentValidation 驗證器設計與同步/非同步驗證
- Test Extensions 的驗證錯誤斷言與條件驗證
- 如何測試時間、角色、條件欄位等複雜驗證情境
- FakeTimeProvider 與驗證器整合測試
