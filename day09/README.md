
# Day 09：範例專案

> Day 09：測試私有與內部成員 - Private 與 Internal 的測試策略

---

## 專案結構

```text
day09/
├── Day09.Samples.sln
├── src/
│   └── Day09.Core/
│       ├── DataProcessor.cs
│       ├── Day09.Core.csproj
│       ├── GlobalUsings.cs
│       ├── Models/
│       │   ├── PaymentMethod.cs
│       │   ├── PaymentRequest.cs
│       │   ├── PaymentResult.cs
│       │   ├── ProcessResult.cs
│       │   └── ValidationResult.cs
│       ├── PaymentProcessor.cs
│       ├── PriceCalculator.cs
│       └── StrategyPattern/
│           ├── Customer.cs
│           ├── IDiscountStrategy.cs
│           ├── ITaxStrategy.cs
│           ├── Location.cs
│           ├── PricingService.cs
│           ├── Product.cs
│           ├── StandardDiscountStrategy.cs
│           └── TaiwanTaxStrategy.cs
└── tests/
		└── Day09.Core.Tests/
				├── DataProcessorTests.cs
				├── Day09.Core.Tests.csproj
				├── GlobalUsings.cs
				├── Helpers/
				│   └── ReflectionTestHelper.cs
				├── PaymentProcessorTests.cs
				├── PriceCalculatorTests.cs
				└── StrategyPattern/
						├── PricingServiceTests.cs
						├── StandardDiscountStrategyTests.cs
						└── TaiwanTaxStrategyTests.cs
```

---

## 專案內容簡介

### src/Day09.Core/

- **DataProcessor.cs**：資料處理器，包含資料驗證、轉換與儲存等流程，示範 protected virtual 方法的測試情境。
- **Day09.Core.csproj**：專案檔，並設定 InternalsVisibleTo，允許測試專案存取 internal 成員。
- **GlobalUsings.cs**：全域 using 設定。
- **Models/**
	- **PaymentMethod.cs**：定義付款方式的列舉型別。
	- **PaymentRequest.cs**：付款請求資料模型。
	- **PaymentResult.cs**：付款結果資料模型，含成功/失敗靜態工廠方法。
	- **ProcessResult.cs**：資料處理結果模型，含成功/失敗靜態工廠方法。
	- **ValidationResult.cs**：資料驗證結果模型。
- **PaymentProcessor.cs**：付款處理器，包含私有驗證與計算手續費邏輯，示範如何測試 private 方法。
- **PriceCalculator.cs**：價格計算器（internal），提供價格等級與折扣計算，示範 internal 類別的測試。
- **StrategyPattern/**
	- **Customer.cs**：顧客資料模型，含 VIP 屬性與地區資訊。
	- **IDiscountStrategy.cs**：折扣計算策略介面。
	- **ITaxStrategy.cs**：稅額計算策略介面。
	- **Location.cs**：地區資料模型。
	- **PricingService.cs**：定價服務，結合折扣與稅額策略計算最終價格。
	- **Product.cs**：商品資料模型。
	- **StandardDiscountStrategy.cs**：標準折扣策略，VIP 顧客享 10% 折扣。
	- **TaiwanTaxStrategy.cs**：台灣稅額策略，商品加收 5% 稅金。

### tests/Day09.Core.Tests/

- **DataProcessorTests.cs**：針對 DataProcessor 進行單元測試，驗證資料處理流程與 protected 方法的覆寫測試。
- **Day09.Core.Tests.csproj**：測試專案檔，參考主程式專案並引入必要測試套件。
- **GlobalUsings.cs**：全域 using 設定，包含 xUnit、NSubstitute、AwesomeAssertions 等。
- **Helpers/ReflectionTestHelper.cs**：反射測試輔助類別，提供呼叫 private/靜態 private 方法的泛型工具。
- **PaymentProcessorTests.cs**：針對 PaymentProcessor 進行單元測試，驗證公開方法與私有驗證/計算邏輯（透過反射）。
- **PriceCalculatorTests.cs**：針對 internal 類別 PriceCalculator 進行單元測試，驗證價格等級與折扣計算。
- **StrategyPattern/**
	- **PricingServiceTests.cs**：測試 PricingService，驗證折扣與稅額策略組合下的最終價格計算。
	- **StandardDiscountStrategyTests.cs**：測試標準折扣策略，驗證 VIP 與一般顧客的折扣行為。
	- **TaiwanTaxStrategyTests.cs**：測試台灣稅額策略，驗證不同金額下的稅額計算。

---

此資料夾為 Day 09 的範例專案，本專案聚焦於 C# 測試私有 (private) 與內部 (internal) 成員的策略，包含反射測試、InternalsVisibleTo、策略模式等實務範例。
