# Day 13 – 範例專案

> [Day 13 – NSubstitute 與 AutoFixture 的整合應用](https://ithelp.ithome.com.tw/articles/10375419)

## 專案結構

```
day13/
├── Day13.Samples.sln
├── src/
│   └── Day13.Core/
│       ├── Day13.Core.csproj
│       ├── Dto/
│       │   ├── ShipperDto.cs
│       │   └── ShipperModel.cs
│       ├── Entities/
│       │   └── ShipperModel.cs
│       ├── MapConfig/
│       │   └── ServiceMapRegister.cs
│       ├── Misc/
│       │   ├── IResult.cs
│       │   └── Result.cs
│       ├── Models/
│       │   └── ShipperModel.cs
│       ├── Repositories/
│       │   └── IShipperRepository.cs
│       ├── Services/
│       │   ├── IShipperService.cs
│       │   └── ShipperService.cs
│       └── Validation/
│           └── ModelValidator.cs
└── tests/
    └── Day13.Core.Tests/
        ├── Day13.Core.Tests.csproj
        ├── GlobalUsings.cs
        ├── AutoDataAttributeUsageTests.cs
        ├── ShipperServiceAdvancedTests.cs
        ├── ShipperServiceBasicTests.cs
        ├── ShipperServiceComplexDataTests.cs
        ├── ShipperServiceParameterizedTests.cs
        ├── ShipperServiceTests.cs
        ├── TraditionalVsAutoNSubstituteTests.cs
        ├── Attributes/
        │   └── CollectionSizeAttribute.cs
        └── AutoFixtureConfigurations/
            ├── AutoDataWithCustomizationAttribute.cs
            ├── InlineAutoDataWithCustomizationAttribute.cs
            └── MapsterMapperCustomization.cs
```

## 專案內容說明

### src/Day13.Core 專案

- **Day13.Core.csproj**  
  Day13 核心專案的專案檔。

#### Dto/
- **ShipperDto.cs**  
  定義 Shipper 的資料傳輸物件 (DTO)，用於資料交換。
- **ShipperModel.cs**  
  可能為 DTO 層的 Shipper 資料模型，與 Entities/Models 可能有不同用途。

#### Entities/
- **ShipperModel.cs**  
  實體層的 Shipper 資料模型，對應資料庫或核心業務邏輯。

#### MapConfig/
- **ServiceMapRegister.cs**  
  註冊服務與物件映射設定，通常用於 DI 或 AutoMapper/Mapster 設定。

#### Misc/
- **IResult.cs**  
  定義操作結果的介面，標準化回傳格式。
- **Result.cs**  
  IResult 的實作，封裝操作結果、訊息與狀態。

#### Models/
- **ShipperModel.cs**  
  一般資料模型，可能用於業務邏輯或資料傳遞。

#### Repositories/
- **IShipperRepository.cs**  
  Shipper 資料存取層的介面，定義 CRUD 操作。

#### Services/
- **IShipperService.cs**  
  Shipper 服務層介面，定義業務邏輯操作。
- **ShipperService.cs**  
  IShipperService 的實作，處理 Shipper 相關業務邏輯。

#### Validation/
- **ModelValidator.cs**  
  資料模型驗證邏輯，確保資料正確性。

### tests/Day13.Core.Tests 測試專案

- **Day13.Core.Tests.csproj**  
  測試專案的專案檔。
- **GlobalUsings.cs**  
  全域 using 設定，簡化測試檔案引用。

#### 測試類別

- **AutoDataAttributeUsageTests.cs**  
  展示 AutoFixture 與自訂 AutoData 屬性的應用。
- **ShipperServiceAdvancedTests.cs**  
  進階測試 ShipperService，涵蓋複雜情境與自訂 mock。
- **ShipperServiceBasicTests.cs**  
  基本單元測試，驗證 ShipperService 的主要功能。
- **ShipperServiceComplexDataTests.cs**  
  測試複雜資料結構與物件圖的處理。
- **ShipperServiceParameterizedTests.cs**  
  參數化測試，驗證多組輸入下的行為。
- **ShipperServiceTests.cs**  
  一般 ShipperService 單元測試，涵蓋常見情境。
- **TraditionalVsAutoNSubstituteTests.cs**  
  傳統 NSubstitute 與 AutoFixture 整合的對照測試。

#### Attributes/
- **CollectionSizeAttribute.cs**  
  自訂集合大小的測試屬性，輔助產生指定長度的集合資料。

#### AutoFixtureConfigurations/
- **AutoDataWithCustomizationAttribute.cs**  
  自訂 AutoData 屬性，結合特定 Customization。
- **InlineAutoDataWithCustomizationAttribute.cs**  
  支援 InlineData 與 AutoData 結合的自訂屬性。
- **MapsterMapperCustomization.cs**  
  AutoFixture 的 Mapster 映射自訂設定。

---

## 主題簡介與學習重點

本日範例專案聚焦於 NSubstitute 與 AutoFixture 的整合應用，透過自訂屬性與 Customization，讓測試資料產生與 mock 更自動化、彈性。  
涵蓋進階 mock、複雜資料結構、參數化測試等實務技巧，並對比傳統與自動化 mock 的差異，提升單元測試的可維護性與可讀性。
