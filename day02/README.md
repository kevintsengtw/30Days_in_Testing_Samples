# Day 02： 範例專案

[Day 02：xUnit 框架深度解析 - 從生態概觀到實戰專案](https://ithelp.ithome.com.tw/articles/10373952)

---

## 專案結構

```text
day02/
├── Day02.Samples.sln
├── src/
│   └── Day02.Core/
│       ├── Calculator.cs
│       ├── Day02.Core.csproj
│       ├── bin/
│       └── obj/
└── tests/
    └── Day02.Core.Tests/
        ├── CalculatorTests.cs
        ├── Day02.Core.Tests.csproj
        ├── bin/
        └── obj/
```

---

## 專案內容簡介

### src/Day02.Core/

- **Calculator.cs**：簡單的計算機類別，提供加法等基本運算方法。

### tests/Day02.Core.Tests/

- **CalculatorTests.cs**：針對 Calculator 進行單元測試，涵蓋加法的多種情境（正數、負數、零等），並示範 xUnit 的 `[Fact]` 及 `[Theory]` 屬性用法，強調測試的自動化與可重複性。

---

此資料夾為 Day 02 的範例專案，重點在於介紹 xUnit 測試框架的基本用法與實戰技巧。
