# OrderProcessor 測試情境分析

## CancelOrderAsync 方法

### 1. 方法簽名分析

- **參數類型**：
  - `orderId`：int（訂單識別碼）
- **回傳類型**：Task\<Order\>（取消後的訂單）
- **是否為非同步**：是（非同步方法）

### 2. 業務邏輯分析

- **資料驗證**：
  - 從資料庫根據 orderId 取得訂單
  - 檢查訂單是否存在
- **業務規則驗證**：
  - 已付款的訂單無法取消（OrderStatus.Paid）
  - 已取消的訂單無法重複取消（OrderStatus.Cancelled）
- **狀態更新**：
  - 將訂單狀態設為 OrderStatus.Cancelled
  - 保存更新後的訂單到資料庫
- **異常處理**：
  - 訂單不存在時拋出 ArgumentException
  - 訂單狀態不允許取消時拋出 InvalidOperationException

### 3. 測試情境建議

**正常情境**：

- **情境1**：取消待處理狀態的訂單
  - 輸入：orderId = 1，訂單狀態 = Pending
  - 預期結果：訂單狀態更新為 Cancelled，回傳更新後的訂單
  - 驗證重點：正常取消流程和狀態轉換

- **情境2**：取消已確認狀態的訂單
  - 輸入：orderId = 2，訂單狀態 = Confirmed
  - 預期結果：訂單狀態更新為 Cancelled，回傳更新後的訂單
  - 驗證重點：已確認訂單的正常取消處理

**邊界條件**：

- **情境3**：使用最小有效訂單編號
  - 輸入：orderId = 1，有效的待處理訂單
  - 預期結果：成功取消訂單
  - 驗證重點：邊界值訂單編號的處理

- **情境4**：使用大數值訂單編號
  - 輸入：orderId = 999999，有效的待處理訂單
  - 預期結果：成功取消訂單
  - 驗證重點：大數值訂單編號的處理穩定性

**異常處理**：

- **情境5**：訂單不存在
  - 輸入：orderId = 999，資料庫中不存在此訂單
  - 預期結果：拋出 ArgumentException，錯誤訊息為「訂單不存在」
  - 驗證重點：不存在訂單的例外處理和錯誤訊息

- **情境6**：嘗試取消已付款的訂單
  - 輸入：orderId = 3，訂單狀態 = Paid
  - 預期結果：拋出 InvalidOperationException，錯誤訊息為「已付款的訂單無法取消」
  - 驗證重點：已付款訂單的業務規則驗證

- **情境7**：嘗試取消已取消的訂單
  - 輸入：orderId = 4，訂單狀態 = Cancelled
  - 預期結果：拋出 InvalidOperationException，錯誤訊息為「訂單已被取消」
  - 驗證重點：重複取消的業務規則驗證

- **情境8**：資料庫操作失敗
  - 輸入：orderId = 5，有效的待處理訂單，但保存時發生異常
  - 預期結果：拋出相應的資料庫異常
  - 驗證重點：資料庫操作失敗時的異常傳播

- **情境9**：負數訂單編號
  - 輸入：orderId = -1
  - 預期結果：由於訂單不存在，拋出 ArgumentException
  - 驗證重點：無效輸入參數的處理

- **情境10**：零值訂單編號
  - 輸入：orderId = 0
  - 預期結果：由於訂單不存在，拋出 ArgumentException
  - 驗證重點：邊界無效值的處理

### 4. 依賴關係識別

**外部依賴**：

- **IOrderRepository**：需要 Mock，用於訂單的查詢（GetByIdAsync）和保存（SaveAsync）操作
- Mock 設定需求：
  - GetByIdAsync 方法：設定不同的回傳值模擬各種訂單狀態和不存在的情況
  - SaveAsync 方法：設定正常回傳和異常情況的模擬

---

## ProcessOrderAsync 方法

### 1. 方法簽名分析

- **參數類型**：
  - `order`：Order（訂單資料物件）
- **回傳類型**：Task\<Order\>（處理後的訂單）
- **是否為非同步**：是（非同步方法）

### 2. 業務邏輯分析

- **輸入驗證**：
  - 檢查訂單物件是否為 null
- **庫存管理流程**：
  - 檢查庫存可用性（CheckAvailabilityAsync）
  - 保留庫存（ReserveStockAsync）
- **付款處理流程**：
  - 處理付款（ProcessPaymentAsync）
  - 檢查付款結果的成功狀態
- **訂單狀態更新**：
  - 將訂單狀態設為 OrderStatus.Confirmed
  - 保存更新後的訂單到資料庫
- **通知流程**：
  - 發送訂單確認通知（SendOrderConfirmationAsync）
- **異常處理**：
  - 訂單為 null 時拋出 ArgumentNullException
  - 庫存不足時拋出 InvalidOperationException
  - 無法保留庫存時拋出 InvalidOperationException
  - 付款失敗時拋出 InvalidOperationException

### 3. 測試情境建議

**正常情境**：

- **情境1**：完整成功的訂單處理流程
  - 輸入：有效的訂單物件，庫存充足，付款成功
  - 預期結果：訂單狀態更新為 Confirmed，回傳更新後的訂單
  - 驗證重點：完整業務流程執行和方法呼叫順序

- **情境2**：多種付款方式的訂單處理
  - 輸入：不同付款方式的訂單物件（CreditCard, BankTransfer, Cash）
  - 預期結果：成功處理並確認訂單
  - 驗證重點：不同付款方式的處理邏輯

- **情境3**：不同數量的商品訂單處理
  - 輸入：不同數量的訂單（1件、多件、大量）
  - 預期結果：正確處理庫存檢查和保留
  - 驗證重點：數量參數的正確傳遞

**邊界條件**：

- **情境4**：最小金額訂單處理
  - 輸入：金額為 0.01 的訂單
  - 預期結果：成功處理極小金額的訂單
  - 驗證重點：小數精度處理和邊界值

- **情境5**：大額訂單處理
  - 輸入：高金額的訂單
  - 預期結果：成功處理大額付款
  - 驗證重點：大數值金額的穩定性

- **情境6**：庫存剛好足夠的情況
  - 輸入：需求數量剛好等於可用庫存
  - 預期結果：成功保留庫存並處理訂單
  - 驗證重點：邊界庫存數量的處理

**異常處理**：

- **情境7**：null 訂單輸入
  - 輸入：order = null
  - 預期結果：拋出 ArgumentNullException
  - 驗證重點：null 參數驗證和錯誤訊息

- **情境8**：庫存不足情況
  - 輸入：有效訂單，但庫存檢查回傳 false
  - 預期結果：拋出 InvalidOperationException，錯誤訊息為「庫存不足」
  - 驗證重點：庫存不足的業務規則驗證

- **情境9**：無法保留庫存情況
  - 輸入：庫存檢查成功，但保留庫存失敗
  - 預期結果：拋出 InvalidOperationException，錯誤訊息為「無法保留庫存」
  - 驗證重點：庫存保留失敗的處理

- **情境10**：付款失敗情況
  - 輸入：庫存操作成功，但付款處理失敗
  - 預期結果：拋出 InvalidOperationException，包含付款錯誤訊息
  - 驗證重點：付款失敗的異常處理和錯誤訊息格式

- **情境11**：訂單保存失敗
  - 輸入：所有前置作業成功，但訂單保存時發生異常
  - 預期結果：拋出相應的資料庫異常
  - 驗證重點：資料庫操作失敗時的異常傳播

- **情境12**：通知發送失敗
  - 輸入：訂單處理成功，但通知發送失敗
  - 預期結果：應正常回傳訂單，不因通知失敗而中斷
  - 驗證重點：通知失敗不影響主要業務流程

**業務流程順序驗證**：

- **情境13**：驗證方法呼叫順序
  - 輸入：正常訂單
  - 預期結果：按正確順序呼叫各依賴服務方法
  - 驗證重點：CheckAvailabilityAsync → ReserveStockAsync → ProcessPaymentAsync → SaveAsync → SendOrderConfirmationAsync

- **情境14**：付款失敗時的資源清理
  - 輸入：庫存保留成功但付款失敗的訂單
  - 預期結果：拋出付款異常
  - 驗證重點：異常處理中的資源清理邏輯（註解中提及但未實作）

### 4. 依賴關係識別

**外部依賴**：

- **IInventoryService**：需要 Mock，用於庫存檢查（CheckAvailabilityAsync）和庫存保留（ReserveStockAsync）操作
- **IPaymentService**：需要 Mock，用於付款處理（ProcessPaymentAsync）操作
- **IOrderRepository**：需要 Mock，用於訂單保存（SaveAsync）操作
- **INotificationService**：需要 Mock，用於發送訂單確認通知（SendOrderConfirmationAsync）操作

- Mock 設定需求：
  - CheckAvailabilityAsync 方法：設定不同的回傳值模擬庫存充足和不足的情況
  - ReserveStockAsync 方法：設定成功和失敗的回傳值
  - ProcessPaymentAsync 方法：設定 PaymentResult 物件，包含成功和失敗的情況
  - SaveAsync 方法：設定正常回傳和異常情況的模擬
  - SendOrderConfirmationAsync 方法：設定成功和失敗的回傳值