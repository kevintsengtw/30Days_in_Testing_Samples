# GitHub Copilot 測試開發指導

## 開發環境資訊
- .NET 9
- 測試框架：xUnit.v3 (3.0.1)
  - NuGet Packages 安裝:
    - xunit.v3
    - xunit.runner.visualstudio
    - NSubstitute (5.3.0)
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

## Mock 物件設定規範
- 使用 Substitute.For<T>() 建立 Mock 物件
- 使用 Returns() 設定回傳值（包含非同步方法）
- 使用 Received() 驗證方法呼叫
- 使用 DidNotReceive() 驗證方法未被呼叫
- 使用 Throws() 模擬異常情況

## 依賴注入測試結構
- 測試類別建構式中初始化所有 Mock 物件
- Mock 物件設定為 private readonly 欄位
- Mock 物件命名：mock{ServiceName}（例：mockUserRepository）
- 在建構式中建立待測試的服務實例

## 依賴注入測試規範
- 非同步測試使用 async/await 模式
- 依賴互動驗證要完整且精確
- Mock 設定要精確，避免過度設定
- 注意測試的獨立性，避免測試間相互影響
- 涵蓋成功路徑、驗證失敗、業務規則、依賴失敗、邊界條件等測試情境

## 業務流程測試規範
- 使用 Received.InOrder() 驗證方法呼叫順序
- 針對業務流程的每個步驟設計失敗測試
- 驗證業務規則和狀態轉換邏輯
- 測試異常情況下的資料一致性
- 重點關注業務邏輯的完整性而非技術實作細節

## 複雜測試情境設計
- 正常業務流程：完整的步驟執行和結果驗證
- 業務規則驗證：狀態檢查、權限驗證、業務條件
- 異常處理測試：每個失敗點的獨立測試
- 邊界條件測試：業務邏輯的臨界值和特殊狀況
- 狀態一致性測試：確保資料狀態的正確轉換
