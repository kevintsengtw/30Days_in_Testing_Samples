# Day 18 – 範例專案

> [Day 18 – 驗證測試：FluentValidation Test Extensions](https://ithelp.ithome.com.tw/articles/10376147)

---

## 專案結構

```
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
            ├── RoleValidationTests.cs
            ├── UserRegistrationAsyncValidatorTests.cs
            └── UserRegistrationValidatorTests.cs
```

---

## 專案內容說明

### src/Day18.Core

- **Models/UserRegistrationRequest.cs**：使用者註冊請求資料模型，包含帳號、信箱、密碼、生日、角色等欄位。
- **Services/IUserService.cs**：用戶服務介面，提供帳號/信箱可用性查詢，供非同步驗證器使用。
- **Validators/UserRegistrationValidator.cs**：同步驗證器，驗證註冊資料格式、密碼規則、生日、角色等。
- **Validators/UserRegistrationAsyncValidator.cs**：非同步驗證器，結合 IUserService 進行帳號/信箱唯一性驗證。
- **Day18.Core.csproj**：主專案組態檔。

### tests/Day18.Core.Tests

- **Validators/UserRegistrationValidatorTests.cs**：同步驗證器的單元測試，驗證各欄位規則、錯誤訊息、時間相關驗證。
- **Validators/UserRegistrationAsyncValidatorTests.cs**：非同步驗證器的單元測試，驗證帳號/信箱唯一性、非同步驗證流程。
- **Validators/ConditionalValidationTests.cs**：條件式驗證測試，驗證欄位有值時才進行驗證的情境。
- **Validators/RoleValidationTests.cs**：角色欄位驗證測試，驗證角色清單的有效性。
- **Extensions/FakeTimeProviderExtensions.cs**：FakeTimeProvider 的輔助擴充方法，方便設定本地時間。
- **GlobalUsings.cs**：測試專案全域 using 設定。
- **Day18.Core.Tests.csproj**：測試專案組態檔。

---

## 技術重點

- **FluentValidation 驗證器**：同步/非同步驗證規則設計，支援多欄位、條件、角色、時間等複雜情境。
- **Test Extensions 斷言**：可直接驗證欄位錯誤、訊息、條件驗證等，提升測試可讀性。
- **FakeTimeProvider 整合**：可控時間驗證，確保時間相關規則可重現。
- **介面注入**：非同步驗證器透過 IUserService 注入，方便模擬外部查詢。

---

本專案示範如何在 .NET 專案中使用 [FluentValidation](https://fluentvalidation.net/) 及其 Test Extensions 進行驗證邏輯的單元測試。內容包含：

- FluentValidation 驗證器設計與同步/非同步驗證
- Test Extensions 的驗證錯誤斷言與條件驗證
- 如何測試時間、角色、條件欄位等複雜驗證情境
- FakeTimeProvider 與驗證器整合測試
