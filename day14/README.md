# Day 14 – 範例專案

> [# Day 14 – Bogus 入門：與 AutoFixture 的差異比較](https://ithelp.ithome.com.tw/articles/10375501)

---

## 專案結構

```
day14/
├── Day14.Samples.sln
├── src/
│   ├── Day14.Core/
│   │   ├── Day14.Core.csproj
│   │   ├── Extensions/
│   │   │   └── TaiwanDataExtensions.cs
│   │   ├── Generators/
│   │   │   ├── AutoBogusDataGenerator.cs
│   │   │   └── BogusDataGenerator.cs
│   │   ├── Models/
│   │   │   ├── Employee.cs
│   │   │   ├── Order.cs
│   │   │   ├── OrderItem.cs
│   │   │   ├── OrderStatus.cs
│   │   │   ├── Product.cs
│   │   │   ├── Project.cs
│   │   │   ├── TaiwanPerson.cs
│   │   │   └── User.cs
│   │   └── GlobalUsings.cs
│   └── Day14.Demo/
│       ├── Day14.Demo.csproj
│       ├── Demo.cs
│       └── Program.cs
└── tests/
    └── Day14.Core.Tests/
        ├── Day14.Core.Tests.csproj
        ├── GlobalUsings.cs
        ├── BasicTests/
        │   └── BogusBasicTests.cs
        ├── ComparisonTests/
        │   └── BogusVsAutoFixtureTests.cs
        └── PerformanceTests/
            └── PerformanceTests.cs
```

---

## 專案內容說明

### src/Day14.Core

- **Extensions/TaiwanDataExtensions.cs**：台灣本地化資料（縣市、大學等）擴充方法。
- **Generators/AutoBogusDataGenerator.cs**：AutoFixture 與 AutoBogus 的資料產生器，示範如何快速產生隨機物件。
- **Generators/BogusDataGenerator.cs**：Bogus 的資料產生器，提供產品、使用者等多種物件的 Faker 設定。
- **Models/Employee.cs**：員工資訊資料模型。
- **Models/Order.cs**：訂單資訊資料模型。
- **Models/OrderItem.cs**：訂單項目資料模型。
- **Models/OrderStatus.cs**：訂單狀態列舉型別。
- **Models/Product.cs**：產品資訊資料模型。
- **Models/Project.cs**：專案資訊資料模型。
- **Models/TaiwanPerson.cs**：台灣人員資訊資料模型。
- **Models/User.cs**：使用者資訊資料模型。
- **GlobalUsings.cs**：全域 using 設定。

### src/Day14.Demo

- **Demo.cs**：主控台範例，展示 Bogus、AutoBogus、台灣本地化、複雜物件、效能比較等多種資料產生情境。
- **Program.cs**：主程式進入點，呼叫各種 Demo 展示方法。

### tests/Day14.Core.Tests

- **BasicTests/BogusBasicTests.cs**：Bogus 基本功能測試，驗證 Faker 產生的產品、使用者等資料正確性。
- **ComparisonTests/BogusVsAutoFixtureTests.cs**：Bogus 與 AutoFixture 產生資料的差異比較測試，包含資料真實性與效能。
- **PerformanceTests/PerformanceTests.cs**：大量資料產生效能測試，驗證 Bogus 在不同數量下的效能表現。
- **GlobalUsings.cs**：測試專案全域 using 設定。
- **Day14.Core.Tests.csproj**：測試專案組態檔。

---

## 特色說明

- **Bogus**：可產生真實感高的隨機資料，支援本地化（如台灣地區、學校等），適合模擬真實情境。
- **AutoFixture**：快速產生隨機物件，適合單元測試，但資料真實性較低。
- **效能比較**：提供 Bogus 與 AutoFixture 在大量資料產生時的效能測試。
- **本地化資料**：內建台灣常見縣市、大學等資料，方便產生在地化測試資料。

---

## Day14.Demo 執行結果

```text
1. 基本 Bogus 功能示範
─────────────────────
產生的產品資料：
  - Incredible Cotton Ball ($302.11) - Beauty
  - Rustic Fresh Chicken ($801.36) - Industrial
  - Handmade Rubber Soap ($751.10) - Garden

產生的使用者資料：
  - Marcia Lesch (Marcia_Lesch@gmail.com) - IT User
  - Jessica Brown (Jessica_Brown@gmail.com) - Marketing User
  - Neil Oberbrunner (Neil.Oberbrunner67@gmail.com) - IT User

2. 台灣本土化資料示範
─────────────────────
產生的台灣人員資料：
  - Roy Altenwerth
    城市: 桃園市
    大學: 交通大學
    公司: 中華電信
    手機: 0922303608
    身分證: O306373707

  - Betsy Witting
    城市: 新北市
    大學: 交通大學
    公司: 中鋼
    手機: 0902572595
    身分證: S740788832

  - Rhonda Brown
    城市: 台中市
    大學: 交通大學
    公司: 英業達
    手機: 0956111707
    身分證: E617474424

  - Jenny Hilpert
    城市: 新北市
    大學: 中興大學
    公司: 台塑
    手機: 0927774869
    身分證: M238390761

  - Elena McLaughlin
    城市: 新北市
    大學: 中興大學
    公司: 英業達
    手機: 0913572822
    身分證: M409297039

3. 複雜物件關係示範
─────────────────────
產生的員工資料：
  - Stanley Goldner (EMP-FS4217)
    職級: Senior, 年齡: 33, 薪資: $51812
    技能: Azure, MongoDB, Docker, C#, SQL Server, React, Git
    專案經驗 (3 個):
      * Awesome Concrete Hat AI (2022/06 - 2023/04)
        技術: Azure, SQL Server
      * Ergonomic Metal Chair COM (2020/09 - 2025/06)
        技術: MongoDB

  - Bernice Kovacek (EMP-SEH20U)
    職級: Senior, 年齡: 27, 薪資: $53380
    技能: Docker, Vue, JavaScript, AWS, Azure
    專案經驗 (4 個):
      * Handcrafted Soft Pants USB (2024/12 - 2025/06)
        技術: Docker, Azure, JavaScript
      * Fantastic Soft Tuna SCSI (2024/09 - 2025/03)
        技術: Vue

產生的訂單資料：
  - 訂單號碼: ORD-L9FA1Y9E
    客戶: Deborah Kirlin (Deborah.Kirlin@gmail.com)
    狀態: Processing, 日期: 2025/08/20
    項目數量: 4, 總金額: $8194.95

  - 訂單號碼: ORD-1HWF9MHU
    客戶: Ignacio Hayes (Ignacio47@yahoo.com)
    狀態: Pending, 日期: 2025/08/19
    項目數量: 4, 總金額: $8148.53

4. AutoBogus 功能示範
─────────────────────
AutoBogus 產生的使用者：
  - FirstName0343e784-38e3-4067-a9ec-e203585d1709 LastNamed5208a37-2980-415b-8c55-d820d144c64b (Emailceb17641-b889-4a2a-899b-8d27f5ffe7e9)
    年齡: 128, 積分: 91

自訂 AutoFixture 產生的使用者：
  - FirstNamee3f596ac-31d4-4bf6-b519-d67395e77c30 LastName3f2fff71-9891-4558-adad-6f73d4fdc87f (test@example.com) - 年齡: 25

5. 效能比較示範
─────────────────────
Bogus 產生 1000 個使用者耗時: 173 ms
AutoBogus 產生 1000 個使用者耗時: 421 ms
記憶體使用差異: Bogus=14,819,392 bytes

6. 可重現性示範
─────────────────────
第一次產生的產品名稱：
  - Ergonomic Steel Cheese
  - Licensed Fresh Keyboard
  - Gorgeous Granite Bike

第二次產生的產品名稱（相同 seed）：
  - Ergonomic Steel Cheese
  - Licensed Fresh Keyboard
  - Gorgeous Granite Bike

兩次產生結果是否相同: True


按任意鍵結束...
```

---

本專案示範如何使用 [Bogus](https://github.com/bchavez/Bogus) 進行 .NET 測試資料產生，並比較 AutoFixture 與 Bogus 的差異。內容包含：
- Bogus 基本用法
- 台灣本地化資料產生
- 複雜物件關聯資料產生
- AutoFixture 與 Bogus 效能與資料真實性比較
- 可重現性測試
