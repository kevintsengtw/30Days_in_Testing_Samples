# Day 29 – TUnit 進階應用：資料驅動測試與依賴注入深度實戰

> [Day 29 – TUnit 進階應用：資料驅動測試與依賴注入深度實戰](https://ithelp.ithome.com.tw/articles/10377970)

---

## 專案結構

```text
├── Day29.Samples.sln                        # 方案檔
├── src/
│   └── TUnit.Advanced.Core/
│       ├── GlobalUsing.cs                   # 全域 using 設定
│       ├── Models/
│       │   ├── DiscountRule.cs              # 折扣規則模型
│       │   ├── Enums.cs                     # 客戶等級、訂單狀態等 enum 定義
│       │   ├── Order.cs                     # 訂單主體，含金額、折扣、運費等屬性
│       │   └── OrderItem.cs                 # 訂單明細，商品、數量、單價等
│       ├── Services/
│       │   ├── DiscountCalculator.cs        # 折扣計算服務，根據規則與折扣碼計算折扣金額
│       │   ├── ICalculatorServices.cs       # 計算服務介面
│       │   ├── IOrderService.cs             # 訂單服務介面
│       │   ├── IRepositoryServices.cs       # Repository 介面
│       │   ├── OrderService.cs              # 訂單服務實作
│       │   └── ShippingCalculator.cs        # 運費計算服務，依客戶等級與金額判斷運費
│       └── TUnit.Advanced.Core.csproj       # 專案描述與依賴
├── tests/
│   ├── TUnit.Advanced.DataDriven.Tests/
│   │   ├── ClassDataSourceTests.cs          # Data-Driven 測試範例（ClassData）
│   │   ├── GlobalUsing.cs                   # 測試專案全域 using 設定
│   │   ├── MatrixTestsExamples.cs           # Matrix 測試範例，展示多維組合測試
│   │   ├── MethodDataSourceTests.cs         # Data-Driven 測試範例（MethodData）
│   │   ├── Program.cs                       # 測試專案進入點
│   │   ├── TUnit.Advanced.DataDriven.Tests.csproj # 測試專案描述
│   │   └── TestData/
│   │       ├── discount-scenarios.json      # 折扣測試情境資料
│   │       ├── MockDiscountRepository.cs    # 折扣資料 mock，供測試用
│   │       ├── MockLogger.cs                # 測試用 logger mock
│   │       ├── TestDataHelper.cs            # 測試資料輔助工具
│   │       ├── TestLogger.cs                # 測試 logger 實作
│   │       ├── TestOrderFactory.cs          # 測試訂單產生器
│   │       ├── TestProductFactory.cs        # 測試商品產生器
│   │       └── TestUserFactory.cs           # 測試用戶產生器
│   └── TUnit.Advanced.Lifecycle.Tests/
│       ├── ConstructorVsBeforeTestOrder.cs  # 建構式與 BeforeTest 執行順序測試
│       ├── DependencyInjectionTests.cs       # 依賴注入與 mock 測試
│       ├── DisposableTests.cs                # IDisposable 測試
│       ├── GlobalUsings.cs                   # 測試專案全域 using 設定
│       ├── LifecycleTests.cs                 # 測試生命週期管理
│       ├── Program.cs                        # 測試專案進入點
│       ├── PropertiesTests.cs                # 屬性測試
│       └── TUnit.Advanced.Lifecycle.Tests.csproj # 測試專案描述
```

---

## 專案內容簡介

### 1. `src/TUnit.Advanced.Core/`

- **GlobalUsing.cs**：全域 using 設定，簡化程式碼。
- **Models/DiscountRule.cs**：折扣規則模型，定義各種折扣條件。
- **Models/Enums.cs**：客戶等級、訂單狀態等 enum 定義。
- **Models/Order.cs**：訂單主體，包含訂單金額、折扣、運費、總金額等屬性。
- **Models/OrderItem.cs**：訂單明細，記錄商品、數量、單價等。
- **Services/DiscountCalculator.cs**：折扣計算服務，根據訂單與折扣碼計算折扣金額，支援多種折扣規則。
- **Services/ICalculatorServices.cs**：計算服務介面。
- **Services/IOrderService.cs**：訂單服務介面。
- **Services/IRepositoryServices.cs**：Repository 介面。
- **Services/OrderService.cs**：訂單服務實作，整合折扣、運費等邏輯。
- **Services/ShippingCalculator.cs**：運費計算邏輯，依據客戶等級與訂單金額決定運費，支援免運規則。
- **TUnit.Advanced.Core.csproj**：專案描述與套件依賴。

### 2. `tests/TUnit.Advanced.DataDriven.Tests/`

- **ClassDataSourceTests.cs**：
	- 展示 xUnit/TUnit 的 Data-Driven 測試寫法，使用 [ClassData] 提供多組測試資料。
	- 範例：同一測試方法自動跑多組輸入，驗證折扣計算、運費規則。
- **GlobalUsing.cs**：測試專案全域 using 設定。
- **MatrixTestsExamples.cs**：
	- 進階 Matrix 測試範例，展示多維度參數組合自動產生測試案例。
	- 重點：
		- 客戶等級 × 訂單金額、三維組合（含折扣碼）、布林值組合等。
		- 展示如何用 MatrixExclusion 排除不合理組合，避免測試爆炸。
		- 實務建議：組合數量要控管，聚焦核心邏輯。
- **MethodDataSourceTests.cs**：
	- 使用 [MemberData] 進行 Data-Driven 測試，驗證不同資料來源的測試案例。
- **Program.cs**：測試專案進入點。
- **TUnit.Advanced.DataDriven.Tests.csproj**：測試專案描述。
- **TestData/discount-scenarios.json**：折扣測試情境資料。
- **TestData/MockDiscountRepository.cs**：假資料來源，模擬折扣查詢。
- **TestData/MockLogger.cs**：簡易 logger mock，避免測試時寫入真實 log。
- **TestData/TestDataHelper.cs**：產生測試資料的輔助工具。
- **TestData/TestLogger.cs**：自訂 logger 實作，方便驗證 log 行為。
- **TestData/TestOrderFactory.cs**：產生各種訂單測試資料。
- **TestData/TestProductFactory.cs**：產生商品資料。
- **TestData/TestUserFactory.cs**：產生用戶資料。

### 3. `tests/TUnit.Advanced.Lifecycle.Tests/`

- **ConstructorVsBeforeTestOrder.cs**：比較建構式與 BeforeTest 的執行順序。
- **DependencyInjectionTests.cs**：展示如何在測試中注入 mock 服務、替換依賴，驗證 DI 行為。
- **DisposableTests.cs**：IDisposable 測試，驗證資源釋放。
- **GlobalUsings.cs**：測試專案全域 using 設定。
- **LifecycleTests.cs**：測試生命週期管理。
- **Program.cs**：測試專案進入點。
- **PropertiesTests.cs**：屬性測試。
- **TUnit.Advanced.Lifecycle.Tests.csproj**：測試專案描述。

---

## 測試類別重點說明

### TUnit.Advanced.DataDriven.Tests

- **ClassDataSourceTests**：
	- 用 [ClassData] 實作 Data-Driven 測試，讓同一測試方法自動驗證多組資料。
	- 範例：多組訂單金額、折扣碼，驗證折扣計算正確性。
- **MatrixTestsExamples**：
	- 展示 Matrix 測試的多種用法：
		- 客戶等級 × 金額組合（16 組）、三維組合（48 組）、布林值組合。
		- 如何用 MatrixExclusion 排除不合理組合。
		- 實務提醒：組合數量要控管，避免測試爆炸。
	- 主要驗證：
		- 運費計算規則（不同會員、金額、折扣下的運費/免運）
		- 折扣計算與總金額一致性
- **MethodDataSourceTests**：
	- 用 [MemberData] 實作 Data-Driven 測試，驗證不同資料來源的測試案例。
- **DependencyInjectionTests**（於 Lifecycle.Tests）：
	- 展示如何在測試中注入 mock 服務、替換依賴，驗證 DI 行為。
	- 方便驗證 logger、repository 等依賴的行為。

### TUnit.Advanced.Lifecycle.Tests

- **ConstructorVsBeforeTestOrder**：驗證建構式與 BeforeTest 執行順序差異。
- **DependencyInjectionTests**：DI 與 mock 實戰，驗證依賴注入行為。
- **DisposableTests**：IDisposable 測試，確保資源釋放。
- **LifecycleTests**：測試生命週期管理。
- **PropertiesTests**：屬性驗證。

---

## 技術重點

- 進階 Data-Driven 與 Matrix 測試技巧，提升測試覆蓋率與維護性。
- 實務控管組合數量，聚焦核心業務邏輯。
- 測試專案大量使用 mock/factory，確保測試獨立、可重現。
- 適合想學習自動化多組合測試、DI 測試技巧的工程師。

---

本範例專案示範如何用 TUnit 進行進階 Data-Driven 與 Matrix 組合測試，並結合 DI/mocking 技巧，讓測試更全面、更貼近實務需求。
