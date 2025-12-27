# Day 27 – GitHub Copilot 測試實戰：AI 輔助測試開發指南

> [Day 27 – GitHub Copilot 測試實戰：AI 輔助測試開發指南](https://ithelp.ithome.com.tw/articles/10377577)

---

## 專案結構

```text
├── Day27.Core.sln                   # 方案檔
├── .github/
│   └── copilot-instructions.md      # GitHub Copilot 指令設定檔
├── docs/
│   └── testing/                     # 測試文件存放目錄
├── completed/
│   ├── stage1/                      # 階段一練習完成檔案
│   │   ├── copilot-instructions-stage1.md
│   │   ├── DiscountCalculatorTests.cs
│   │   └── DiscountCalculator_TestScenario.md
│   ├── stage2/                      # 階段二練習完成檔案
│   │   ├── copilot-instructions-stage2.md
│   │   ├── UserServiceTests.cs
│   │   └── UserService_TestScenario.md
│   └── stage3/                      # 階段三練習完成檔案
│       ├── copilot-instructions-stage3.md
│       ├── OrderProcessorTests.cs
│       └── OrderProcessor_TestScenario.md
├── src/
│   └── Day27.Core/
│       ├── Day27.Core.csproj        # 核心專案 csproj
│       ├── GlobalUsings.cs          # 全域 using 設定
│       ├── Interfaces/
│       │   ├── IEmailService.cs     # Email 服務介面
│       │   ├── IInventoryService.cs # 庫存服務介面
│       │   ├── INotificationService.cs # 通知服務介面
│       │   ├── IOrderRepository.cs  # 訂單 Repository 介面
│       │   ├── IPaymentService.cs   # 付款服務介面
│       │   └── IUserRepository.cs   # 使用者 Repository 介面
│       ├── Models/
│       │   ├── Order.cs             # 訂單模型
│       │   ├── OrderStatus.cs       # 訂單狀態 enum
│       │   ├── PaymentResult.cs     # 付款結果模型
│       │   ├── RegisterUserRequest.cs # 使用者註冊請求模型
│       │   ├── User.cs              # 使用者模型
│       │   └── UserStatus.cs        # 使用者狀態 enum
│       └── Services/
│           ├── DiscountCalculator.cs # 折扣計算服務
│           ├── OrderProcessor.cs    # 訂單處理服務
│           └── UserService.cs       # 使用者服務
└── tests/
    └── Day27.Core.Tests/
        ├── Day27.Core.Tests.csproj  # 測試專案 csproj（含 xUnit v3）
        ├── GlobalUsings.cs          # 測試專案全域 using 設定
        └── Services/                # 測試檔案存放目錄（空的，供練習用）
```

---

## 專案內容簡介

### 1. `.github/`

- **copilot-instructions.md**：GitHub Copilot 指令設定檔，定義 AI 協助測試開發的規則與風格。

### 2. `docs/testing/`

- 測試文件存放目錄，練習時產生的文件檔案會放在這裡。

### 3. `completed/`

三個學習階段的完成檔案，讀者練習完後可作為比對：

- **stage1/**：
  - **copilot-instructions-stage1.md**：階段一的 Copilot 指令設定。
  - **DiscountCalculatorTests.cs**：折扣計算器測試類別。
  - **DiscountCalculator_TestScenario.md**：折扣計算測試情境說明。
- **stage2/**：
  - **copilot-instructions-stage2.md**：階段二的 Copilot 指令設定。
  - **UserServiceTests.cs**：使用者服務測試類別。
  - **UserService_TestScenario.md**：使用者服務測試情境說明。
- **stage3/**：
  - **copilot-instructions-stage3.md**：階段三的 Copilot 指令設定。
  - **OrderProcessorTests.cs**：訂單處理器測試類別。
  - **OrderProcessor_TestScenario.md**：訂單處理測試情境說明。

### 4. `src/Day27.Core/`

- **Day27.Core.csproj**：核心專案描述與套件依賴。
- **GlobalUsings.cs**：全域 using 設定，簡化程式碼。
- **Interfaces/**：
  - **IEmailService.cs**：Email 服務介面。
  - **IInventoryService.cs**：庫存管理服務介面。
  - **INotificationService.cs**：通知服務介面。
  - **IOrderRepository.cs**：訂單 Repository 介面。
  - **IPaymentService.cs**：付款處理服務介面。
  - **IUserRepository.cs**：使用者 Repository 介面。
- **Models/**：
  - **Order.cs**：訂單資料模型。
  - **OrderStatus.cs**：訂單狀態 enum。
  - **PaymentResult.cs**：付款結果模型。
  - **RegisterUserRequest.cs**：使用者註冊請求 DTO。
  - **User.cs**：使用者資料模型。
  - **UserStatus.cs**：使用者狀態 enum。
- **Services/**：
  - **DiscountCalculator.cs**：折扣計算服務實作。
  - **OrderProcessor.cs**：訂單處理服務實作。
  - **UserService.cs**：使用者服務實作。

### 5. `tests/Day27.Core.Tests/`

- **Day27.Core.Tests.csproj**：測試專案描述，已設定好三個階段練習所需的 NuGet 套件，測試框架使用 xUnit v3。
- **GlobalUsings.cs**：測試專案全域 using 設定。
- **Services/**：測試檔案存放目錄（空的），供讀者練習時使用。

---

## 練習指南

本專案是配合 Day 27 文章的實作練習專案，提供三個漸進式學習階段：

1. **階段一**：基礎 AI 輔助測試 - 折扣計算器測試
2. **階段二**：進階 Mock 與驗證 - 使用者服務測試  
3. **階段三**：複雜整合測試 - 訂單處理器測試

每個階段的完成檔案都放在 `completed/` 目錄中，讀者可以：

- 先自己練習在 `tests/Day27.Core.Tests/Services/` 下建立測試檔案
- 完成後與 `completed/` 中的範例比對
- 參考各階段的 Copilot 指令設定檔來優化 AI 協助效果

---

## 技術重點

- GitHub Copilot AI 輔助測試開發技巧
- xUnit v3 測試框架應用
- Mock 與 Stub 技術實戰
- 測試驅動開發 (TDD) 流程
- 複雜業務邏輯的測試策略
- AI 協助文件產生與測試情境規劃

---

本範例專案示範如何有效運用 GitHub Copilot 進行測試開發，從簡單的單元測試到複雜的整合測試，讓 AI 成為測試開發的得力助手。適合想學習 AI 輔助開發、提升測試開發效率的工程師。
