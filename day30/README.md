# Day 30 – TUnit 進階應用：執行控制、測試品質與 ASP.NET Core 整合測試

> [Day 30 - TUnit 進階應用 - 執行控制與測試品質和 ASP.NET Core 整合測試實戰](https://ithelp.ithome.com.tw/articles/10378176)

---

## 專案結構

```text
├── Day30.Samples.sln                        # 方案檔
├── src/
│   ├── TUnit.Advanced.Core/
│   │   ├── GlobalUsing.cs                   # 全域 using 設定
│   │   ├── Models/
│   │   │   ├── DiscountRule.cs              # 折扣規則模型
│   │   │   ├── Enums.cs                     # enum 定義（客戶等級、訂單狀態等）
│   │   │   ├── Order.cs                     # 訂單主體，含金額、折扣、運費等
│   │   │   └── OrderItem.cs                 # 訂單明細，商品、數量、單價等
│   │   ├── Services/
│   │   │   ├── DiscountCalculator.cs        # 折扣計算服務
│   │   │   ├── ICalculatorServices.cs       # 計算服務介面
│   │   │   ├── IOrderService.cs             # 訂單服務介面
│   │   │   ├── IRepositoryServices.cs       # Repository 介面
│   │   │   ├── OrderService.cs              # 訂單服務實作
│   │   │   └── ShippingCalculator.cs        # 運費計算服務
│   │   └── TUnit.Advanced.Core.csproj       # 專案描述與依賴
│   └── TUnit.Advanced.WebApi/
│       ├── appsettings.Development.json     # 開發環境設定
│       ├── appsettings.json                 # 主要設定檔
│       ├── Program.cs                       # ASP.NET Core 進入點
│       ├── Properties/
│       │   └── launchSettings.json          # 啟動設定
│       ├── TUnit.Advanced.WebApi.csproj     # Web API 專案描述
│       └── TUnit.Advanced.WebApi.http       # HTTP 測試腳本
├── tests/
│   ├── TUnit.Advanced.ExecutionControl.Tests/
│   │   ├── ExecutionControlTests.cs         # 測試執行控制（順序、條件、重試等）
│   │   ├── GlobalUsing.cs                   # 測試專案全域 using 設定
│   │   ├── Program.cs                       # 測試專案進入點
│   │   └── TUnit.Advanced.ExecutionControl.Tests.csproj # 測試專案描述
│   └── TUnit.Advanced.Integration.Tests/
│       ├── AdvancedDependencyTests.cs       # 進階 DI 整合測試
│       ├── BasicIntegrationTests.cs         # 基本 API 整合測試
│       ├── ComplexInfrastructureTests.cs    # 複雜基礎設施整合測試
│       ├── GlobalTestInfrastructureSetup.cs # 全域測試基礎設施初始化
│       ├── GlobalUsings.cs                  # 測試專案全域 using 設定
│       ├── IntegrationTests.cs              # 綜合整合測試
│       ├── OrderApiIntegrationTests.cs      # 訂單 API 整合測試
│       ├── Program.cs                       # 測試專案進入點
│       ├── TestContainersSetup.cs           # Testcontainers 設定
│       ├── TestInfrastructureManager.cs     # 測試基礎設施管理
│       └── TUnit.Advanced.Integration.Tests.csproj # 測試專案描述
```

---

## 專案內容簡介

### 1. `src/TUnit.Advanced.Core/`

- **GlobalUsing.cs**：全域 using 設定，簡化程式碼。
- **Models/**：
  - **DiscountRule.cs**：折扣規則模型。
  - **Enums.cs**：enum 定義（客戶等級、訂單狀態等）。
  - **Order.cs**：訂單主體，包含訂單金額、折扣、運費等。
  - **OrderItem.cs**：訂單明細。
- **Services/**：
  - **DiscountCalculator.cs**：折扣計算服務。
  - **ICalculatorServices.cs**：計算服務介面。
  - **IOrderService.cs**：訂單服務介面。
  - **IRepositoryServices.cs**：Repository 介面。
  - **OrderService.cs**：訂單服務實作。
  - **ShippingCalculator.cs**：運費計算服務。
- **TUnit.Advanced.Core.csproj**：專案描述與依賴。

### 2. `src/TUnit.Advanced.WebApi/`

- **appsettings.Development.json**：開發環境設定。
- **appsettings.json**：主要設定檔。
- **Program.cs**：ASP.NET Core 進入點。
- **Properties/launchSettings.json**：啟動設定。
- **TUnit.Advanced.WebApi.csproj**：Web API 專案描述。
- **TUnit.Advanced.WebApi.http**：HTTP 測試腳本。

### 3. `tests/TUnit.Advanced.ExecutionControl.Tests/`

- **ExecutionControlTests.cs**：
  - 測試 TUnit 執行控制功能（如測試順序、條件執行、重試、逾時等）。
- **GlobalUsing.cs**：測試專案全域 using 設定。
- **Program.cs**：測試專案進入點。
- **TUnit.Advanced.ExecutionControl.Tests.csproj**：測試專案描述。

### 4. `tests/TUnit.Advanced.Integration.Tests/`

- **AdvancedDependencyTests.cs**：
  - 進階 DI 與多層依賴整合測試。
- **BasicIntegrationTests.cs**：
  - 基本 API 整合測試，驗證 Web API 端點與資料流。
- **ComplexInfrastructureTests.cs**：
  - 複雜基礎設施（多容器、多服務）整合測試。
- **GlobalTestInfrastructureSetup.cs**：
  - 全域測試基礎設施初始化，確保測試環境一致。
- **GlobalUsings.cs**：測試專案全域 using 設定。
- **IntegrationTests.cs**：
  - 綜合整合測試，驗證多元情境。
- **OrderApiIntegrationTests.cs**：
  - 訂單 API 端點整合測試。
- **Program.cs**：測試專案進入點。
- **TestContainersSetup.cs**：
  - Testcontainers 設定與啟動。
- **TestInfrastructureManager.cs**：
  - 測試基礎設施管理，協助資源釋放與初始化。
- **TUnit.Advanced.Integration.Tests.csproj**：測試專案描述。

---

## 測試類別重點說明

### TUnit.Advanced.ExecutionControl.Tests

- **ExecutionControlTests**：
  - 驗證 TUnit 的執行控制能力（順序、條件、重試、逾時等），確保測試流程可控且穩定。

### TUnit.Advanced.Integration.Tests

- **BasicIntegrationTests**：
  - 基本 API 整合測試，驗證 Web API 端點正確性。
- **AdvancedDependencyTests**：
  - 進階依賴注入與多層服務整合測試。
- **ComplexInfrastructureTests**：
  - 多容器、多服務的複雜整合測試。
- **OrderApiIntegrationTests**：
  - 訂單 API 端點整合測試。
- **IntegrationTests**：
  - 綜合整合測試，涵蓋多元情境。
- **GlobalTestInfrastructureSetup**、**TestContainersSetup**、**TestInfrastructureManager**：
  - 測試基礎設施初始化、資源管理與 Testcontainers 設定。

---

## 技術重點

- TUnit 進階執行控制（順序、條件、重試、逾時等）
- ASP.NET Core Web API 整合測試
- Testcontainers 多容器、多服務整合
- 全域測試基礎設施初始化與資源管理
- 適合需要高品質、可控測試流程的 .NET 工程師

---

本範例專案示範如何用 TUnit 進行進階執行控制、測試品質提升，以及 ASP.NET Core Web API 的整合測試，並結合 Testcontainers 實現多容器、多服務的真實測試環境。
