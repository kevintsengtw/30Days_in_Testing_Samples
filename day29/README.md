# Day 29 – 進階 Data-Driven 與 Matrix 測試實戰

---

## 專案結構

```text
├── Day29.Samples.sln                  # 方案檔
├── src/
│   └── TUnit.Advanced.DataDriven/
│       ├── DiscountCalculator.cs      # 折扣計算服務，含多種折扣邏輯
│       ├── Order.cs                   # 訂單實體，含金額、折扣、運費等屬性
│       ├── OrderItem.cs               # 訂單項目，單一商品明細
│       ├── ShippingCalculator.cs      # 運費計算服務，依客戶等級與金額判斷運費
│       └── CustomerLevel.cs           # 客戶等級 enum，定義會員分級
├── tests/
│   └── TUnit.Advanced.DataDriven.Tests/
│       ├── ClassDataSourceTests.cs    # 基本 Data-Driven 測試範例
│       ├── MatrixTestsExamples.cs     # Matrix 測試範例，展示多維組合測試
│       ├── TestData/
│       │   ├── MockDiscountRepository.cs # 折扣資料 mock，供測試用
│       │   ├── MockLogger.cs              # 測試用 logger mock
│       │   ├── TestDataHelper.cs          # 測試資料輔助工具
│       │   ├── TestLogger.cs              # 測試 logger 實作
│       │   ├── TestOrderFactory.cs        # 測試訂單產生器
│       │   ├── TestProductFactory.cs      # 測試商品產生器
│       │   └── TestUserFactory.cs         # 測試用戶產生器
│       └── DependencyInjectionTests.cs # DI 測試範例，展示注入與 mock
```

---

## 專案內容簡介

### 1. `src/TUnit.Advanced.DataDriven/`
- **DiscountCalculator.cs**：折扣計算服務，根據訂單與折扣碼計算折扣金額，支援多種折扣規則。
- **Order.cs**：訂單主體，包含訂單金額、折扣、運費、總金額等屬性。
- **OrderItem.cs**：訂單明細，記錄商品、數量、單價等。
- **ShippingCalculator.cs**：運費計算邏輯，依據客戶等級與訂單金額決定運費，支援免運規則。
- **CustomerLevel.cs**：會員等級 enum，定義一般、VIP、白金、鑽石會員。

### 2. `tests/TUnit.Advanced.DataDriven.Tests/`
- **ClassDataSourceTests.cs**：
  - 展示 xUnit/TUnit 的 Data-Driven 測試寫法，使用 [ClassData] 提供多組測試資料。
  - 範例：同一測試方法自動跑多組輸入，驗證折扣計算、運費規則。
- **MatrixTestsExamples.cs**：
  - 進階 Matrix 測試範例，展示多維度參數組合自動產生測試案例。
  - 重點：
    - 客戶等級 × 訂單金額、三維組合（含折扣碼）、布林值組合等。
    - 展示如何用 MatrixExclusion 排除不合理組合，避免測試爆炸。
    - 實務建議：組合數量要控管，聚焦核心邏輯。
- **TestData/**：
  - **MockDiscountRepository.cs**：假資料來源，模擬折扣查詢。
  - **MockLogger.cs**：簡易 logger mock，避免測試時寫入真實 log。
  - **TestDataHelper.cs**：產生測試資料的輔助工具。
  - **TestLogger.cs**：自訂 logger 實作，方便驗證 log 行為。
  - **TestOrderFactory.cs**：產生各種訂單測試資料。
  - **TestProductFactory.cs**：產生商品資料。
  - **TestUserFactory.cs**：產生用戶資料。
- **DependencyInjectionTests.cs**：
  - 展示如何在測試中注入 mock 服務、替換依賴，驗證 DI 行為。

---

## 測試類別重點說明

### MatrixTestsExamples
- 展示 Matrix 測試的多種用法：
  - 客戶等級 × 金額組合（16 組）、三維組合（48 組）、布林值組合。
  - 如何用 MatrixExclusion 排除不合理組合。
  - 實務提醒：組合數量要控管，避免測試爆炸。
- 主要驗證：
  - 運費計算規則（不同會員、金額、折扣下的運費/免運）
  - 折扣計算與總金額一致性

### ClassDataSourceTests
- 用 [ClassData] 實作 Data-Driven 測試，讓同一測試方法自動驗證多組資料。
- 範例：多組訂單金額、折扣碼，驗證折扣計算正確性。

### DependencyInjectionTests
- 展示如何在測試中注入 mock 服務、替換依賴，驗證 DI 行為。
- 方便驗證 logger、repository 等依賴的行為。

---

## 技術重點
- 進階 Data-Driven 與 Matrix 測試技巧，提升測試覆蓋率與維護性。
- 實務控管組合數量，聚焦核心業務邏輯。
- 測試專案大量使用 mock/factory，確保測試獨立、可重現。
- 適合想學習自動化多組合測試、DI 測試技巧的工程師。

---

本範例專案示範如何用 TUnit 進行進階 Data-Driven 與 Matrix 組合測試，並結合 DI/mocking 技巧，讓測試更全面、更貼近實務需求。
