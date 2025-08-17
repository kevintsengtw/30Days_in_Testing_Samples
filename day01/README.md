# Day 01：範例專案

[Day 01：老派工程師的測試啟蒙 - 為什麼我們需要測試？](https://ithelp.ithome.com.tw/articles/10373888)

---

## 專案結構

```
day01/
├── Day01.Samples.sln
├── src/
│   └── Day01.Core/
│       ├── Calculator.cs
│       ├── Counter.cs
│       ├── Day01.Core.csproj
│       ├── EmailHelper.cs
│       ├── OrderService.cs
│       ├── PriceCalculator.cs
│       ├── bin/
│       ├── Enums/
│       ├── Models/
│       └── obj/
└── tests/
    └── Day01.Samples.Tests/
        ├── CalculatorTests.cs
        ├── CounterTests.cs
        ├── Day01.Samples.Tests.csproj
        ├── EmailHelperTests.cs
        ├── OrderServiceTests.cs
        ├── PriceCalculatorTests.cs
        ├── bin/
        └── obj/
```

---

此資料夾為 Day 01 的範例專案，內容包含核心程式碼與對應的單元測試。

## 專案內容簡介

### src/Day01.Core/
* **Calculator.cs**：簡單的計算機類別，提供加法等基本運算。
* **Counter.cs**：計數器類別，支援遞增操作。
* **EmailHelper.cs**：Email 格式驗證輔助類別。
* **OrderService.cs**：訂單處理服務，模擬業務邏輯。
* **PriceCalculator.cs**：價格計算器，支援折扣計算。

### tests/Day01.Samples.Tests/
* **CalculatorTests.cs**：針對 Calculator 進行單元測試，展示 FIRST 原則（快速、獨立、可重複、可自我驗證、及時）。
* **CounterTests.cs**：針對 Counter 進行單元測試，強調測試的獨立性與可重複性。
* **EmailHelperTests.cs**：針對 Email 格式驗證進行測試，涵蓋有效/無效 Email、null 等情境。
* **OrderServiceTests.cs**：針對訂單服務的處理流程進行測試，驗證訂單處理的正確性與例外處理。
* **PriceCalculatorTests.cs**：針對價格計算與折扣邏輯進行測試，包含正常與異常輸入的驗證。

---

每個測試類別皆以簡單明確的 Arrange-Act-Assert 結構撰寫，並強調單元測試的良好實踐原則。
