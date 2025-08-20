# Day 11 – 範例專案

> [Day 11 – AutoFixture 進階：自訂化測試資料生成策略](https://ithelp.ithome.com.tw/articles/10375153)

## 專案結構

```
day11/
├── Day11.Samples.sln
├── src/
│   └── Day11.Core/
│       ├── Day11.Core.csproj
│       ├── GlobalUsings.cs
│       ├── Member.cs
│       ├── Order.cs
│       ├── Person.cs
│       └── Product.cs
└── tests/
    └── Day11.Core.Tests/
        ├── Day11.Core.Tests.csproj
        ├── GlobalUsings.cs
        ├── DataAnnotationsTests.cs
        ├── DateTimeRangeTests.cs
        ├── GenericNumericRangeBuilderTests.cs
        ├── NumericRangeBuilderTests.cs
        ├── PropertyRangeTests.cs
        ├── RandomRangedNumericSequenceBuilderTests.cs
        ├── WithBehaviorTests.cs
        └── TestHelpers/
            ├── FixtureRangedNumericExtensions.cs
            ├── ImprovedRandomRangedNumericSequenceBuilder.cs
            ├── NumericRangeBuilder.cs
            ├── RandomRangedDateTimeBuilder.cs
            └── RandomRangedNumericSequenceBuilder.cs
```

---

## 專案內容說明

### 方案與專案

- **Day11.Samples.sln**  
  Visual Studio 方案檔，整合 src 與 tests 兩個專案。

#### src/Day11.Core

- **Day11.Core.csproj**  
  主程式庫專案檔，目標 .NET 9，無特殊依賴。
- **GlobalUsings.cs**  
  全域 using 設定，方便專案內引用 DataAnnotations。
- **Member.cs**  
  會員類別，示範屬性加上 `[Range]` 等 DataAnnotations 限制。
- **Order.cs**  
  訂單類別，含多種數值型屬性，展示泛型範圍控制。
- **Person.cs**  
  人員類別，含 Guid、Name、Age 等，示範多種 DataAnnotations。
- **Product.cs**  
  產品類別，含價格、庫存等數值屬性，展示範圍控制。

#### tests/Day11.Core.Tests

- **Day11.Core.Tests.csproj**  
  測試專案檔，參考 AutoFixture、xUnit、AwesomeAssertions 等套件。
- **GlobalUsings.cs**  
  測試專案全域 using 設定。
- **DataAnnotationsTests.cs**  
  測試 AutoFixture 是否能自動產生符合 DataAnnotations 限制的資料。
- **DateTimeRangeTests.cs**  
  測試自訂 DateTime 範圍生成器，控制日期屬性生成範圍。
- **GenericNumericRangeBuilderTests.cs**  
  測試泛型數值範圍生成器，支援多型別屬性範圍控制。
- **NumericRangeBuilderTests.cs**  
  測試數值範圍生成器的功能與優先順序問題。
- **PropertyRangeTests.cs**  
  測試 `.With()` 方法動態控制屬性值範圍。
- **RandomRangedNumericSequenceBuilderTests.cs**  
  測試自訂數值範圍生成器的行為與限制。
- **WithBehaviorTests.cs**  
  比較 `.With()` 固定值與動態值（lambda）的差異。

##### TestHelpers/ (測試輔助類別)

- **FixtureRangedNumericExtensions.cs**  
  擴充 AutoFixture，提供泛型數值範圍設定的便利方法。
- **ImprovedRandomRangedNumericSequenceBuilder.cs**  
  改良版數值範圍生成器，支援 predicate 精確控制。
- **NumericRangeBuilder.cs**  
  泛型數值範圍生成器，支援所有數值型別。
- **RandomRangedDateTimeBuilder.cs**  
  自訂 DateTime 範圍生成器，可指定目標屬性。
- **RandomRangedNumericSequenceBuilder.cs**  
  第一版數值範圍生成器，簡單屬性名稱比對。

---

## 測試類別重點說明

- **DataAnnotationsTests**  
  驗證 AutoFixture 能否自動產生符合 DataAnnotations（如 StringLength、Range）規範的資料。
- **DateTimeRangeTests**  
  示範如何自訂 DateTime 範圍生成器，產生指定區間的日期屬性。
- **GenericNumericRangeBuilderTests**  
  利用擴充方法，針對不同型別屬性設定數值範圍，提升測試彈性。
- **NumericRangeBuilderTests**  
  探討數值範圍生成器的優先順序與覆蓋問題。
- **PropertyRangeTests**  
  使用 `.With()` 方法直接指定屬性值範圍，靈活控制測試資料。
- **RandomRangedNumericSequenceBuilderTests**  
  測試自訂數值範圍生成器的行為與 AutoFixture 內建生成器的衝突。
- **WithBehaviorTests**  
  比較 `.With()` 固定值與 lambda 動態值的差異，說明生成多筆資料時的行為。

---

本資料夾為 Day 11 範例專案，示範如何在 .NET 測試中，利用 AutoFixture 進行進階的自訂化測試資料生成，包含數值範圍、屬性條件、DataAnnotations 整合等。