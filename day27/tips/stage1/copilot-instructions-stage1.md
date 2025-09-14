# GitHub Copilot 測試開發指導

## 開發環境資訊
- .NET 9
- 測試框架：xUnit.v3 (3.0.1)
  - NuGet Packages 安裝:
    - xunit.v3
    - xunit.runner.visualstudio
- 斷言庫：AwesomeAssertions (不使用 FluentAssertions)

## 測試檔案組織結構
- 測試專案命名：{ProjectName}.Tests
- 測試類別命名：{ClassName}Tests

## 測試方法命名規範（重要）
- **測試方法名稱必須使用繁體中文**
- 命名格式：方法名_測試情境_預期結果
- 範例：
  - CalculateDiscountedPrice_輸入有效價格和折扣率_應回傳正確折扣價格()
  - CalculateDiscountedPrice_輸入負數原始價格_應拋出ArgumentException()

## DisplayName 使用規範（重要）
- **每個測試方法都必須在 [Fact] 屬性中指定 DisplayName 參數**
- DisplayName 格式：`方法功能描述: 情境描述，預期結果`
- 範例：
  [Fact(DisplayName = "計算折扣後的價格: 輸入有效的價格和折扣率，應回傳正確的折扣價格")]
  public void CalculateDiscountedPrice_輸入有效價格和折扣率_應回傳正確折扣價格()

## 測試程式碼規範
- 所有測試必須遵循 AAA 模式 (Arrange-Act-Assert)
- 必須標註 // Arrange, // Act, // Assert 三個區塊
- 使用 AwesomeAssertions 的 Should() 語法進行斷言
- 變數名稱使用英文，類別、方法、屬性註解使用繁體中文

## 程式碼風格要求
- 使用繁體中文進行測試方法命名和 DisplayName
- 類別、建構式、屬性、方法以及程式註解一律使用繁體中文
- 變數名稱、函數名稱、類別名稱一律使用英文
- if 判斷式一定要有大括號 {}, 即使只有一層也不能省略
- 一個類別就是一個檔案，不要所有類別都放在同一個 cs 檔案裡