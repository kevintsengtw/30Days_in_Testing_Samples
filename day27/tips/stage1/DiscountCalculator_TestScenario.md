# DiscountCalculator 測試情境分析

## CalculateDiscountedPrice 方法

### 1. 方法簽名分析

- **參數類型**：
  - `originalPrice`：decimal（原始價格）
  - `discountRate`：decimal（折扣率）
- **回傳類型**：decimal（折扣後的價格）
- **是否為非同步**：否（同步方法）

### 2. 業務邏輯分析

- **輸入驗證**：
  - 原始價格必須大於等於 0
  - 折扣率必須在 0.0 到 1.0 之間（包含邊界值）
- **計算邏輯**：
  - 折扣後價格 = 原始價格 × (1 - 折扣率)
- **異常處理**：
  - 原始價格為負數時拋出 ArgumentException
  - 折扣率小於 0 或大於 1 時拋出 ArgumentException

### 3. 測試情境建議

**正常情境**：

- **情境1**：一般折扣計算
  - 輸入：originalPrice = 100m, discountRate = 0.2m
  - 預期結果：80m
  - 驗證重點：基本折扣計算邏輯正確性

- **情境2**：無折扣計算
  - 輸入：originalPrice = 50m, discountRate = 0m
  - 預期結果：50m
  - 驗證重點：零折扣率時價格不變

- **情境3**：全額折扣計算
  - 輸入：originalPrice = 100m, discountRate = 1m
  - 預期結果：0m
  - 驗證重點：100% 折扣時價格為零

- **情境4**：小數價格計算
  - 輸入：originalPrice = 99.99m, discountRate = 0.15m
  - 預期結果：84.9915m
  - 驗證重點：小數精度計算正確性

- **情境5**：小數折扣率計算
  - 輸入：originalPrice = 100m, discountRate = 0.333m
  - 預期結果：66.7m
  - 驗證重點：小數折扣率計算精度

**邊界條件**：

- **情境6**：零價格輸入
  - 輸入：originalPrice = 0m, discountRate = 0.5m
  - 預期結果：0m
  - 驗證重點：零價格邊界值處理

- **情境7**：最小折扣率邊界
  - 輸入：originalPrice = 100m, discountRate = 0.0m
  - 預期結果：100m
  - 驗證重點：最小折扣率邊界處理

- **情境8**：最大折扣率邊界
  - 輸入：originalPrice = 100m, discountRate = 1.0m
  - 預期結果：0m
  - 驗證重點：最大折扣率邊界處理

- **情境9**：極小價格值
  - 輸入：originalPrice = 0.01m, discountRate = 0.1m
  - 預期結果：0.009m
  - 驗證重點：極小數值計算精度

- **情境10**：極大價格值
  - 輸入：originalPrice = 999999.99m, discountRate = 0.1m
  - 預期結果：899999.991m
  - 驗證重點：大數值計算穩定性

**異常處理**：

- **情境11**：負數原始價格
  - 輸入：originalPrice = -100m, discountRate = 0.2m
  - 預期結果：拋出 ArgumentException
  - 驗證重點：負數價格驗證和錯誤訊息

- **情境12**：負數折扣率
  - 輸入：originalPrice = 100m, discountRate = -0.1m
  - 預期結果：拋出 ArgumentException
  - 驗證重點：負數折扣率驗證和錯誤訊息

- **情境13**：超過1的折扣率
  - 輸入：originalPrice = 100m, discountRate = 1.5m
  - 預期結果：拋出 ArgumentException
  - 驗證重點：超範圍折扣率驗證和錯誤訊息

- **情境14**：極端負數價格
  - 輸入：originalPrice = -999999.99m, discountRate = 0.1m
  - 預期結果：拋出 ArgumentException
  - 驗證重點：極端負數輸入處理

### 4. 依賴關係識別

**無外部依賴**：此方法為純函數，僅依賴輸入參數進行計算，不需要 Mock 任何外部服務或依賴項目。

---

## CalculateBulkDiscount 方法

### 1. 方法簽名分析

- **參數類型**：
  - `originalPrice`：decimal（原始價格）
  - `quantity`：int（數量）
- **回傳類型**：decimal（批量折扣後的總價）
- **是否為非同步**：否（同步方法）

### 2. 業務邏輯分析

- **輸入驗證**：
  - 原始價格必須大於等於 0
  - 數量必須大於 0
- **計算邏輯**：
  - 總價 = 原始價格 × 數量
  - 根據數量決定批量折扣率：數量 >= 100 為 15% 折扣，>= 50 為 10% 折扣，>= 10 為 5% 折扣，< 10 無折扣
  - 調用 CalculateDiscountedPrice 方法計算最終折扣價格
- **異常處理**：
  - 原始價格為負數時拋出 ArgumentException
  - 數量小於等於 0 時拋出 ArgumentException

### 3. 測試情境建議

**正常情境**：

- **情境1**：小量購買（無折扣）
  - 輸入：originalPrice = 10m, quantity = 5
  - 預期結果：50m
  - 驗證重點：數量小於 10 時無折扣

- **情境2**：中等量購買（5% 折扣）
  - 輸入：originalPrice = 10m, quantity = 15
  - 預期結果：142.5m
  - 驗證重點：數量 >= 10 且 < 50 時 5% 折扣

- **情境3**：大量購買（10% 折扣）
  - 輸入：originalPrice = 10m, quantity = 60
  - 預期結果：540m
  - 驗證重點：數量 >= 50 且 < 100 時 10% 折扣

- **情境4**：批量購買（15% 折扣）
  - 輸入：originalPrice = 10m, quantity = 120
  - 預期結果：1020m
  - 驗證重點：數量 >= 100 時 15% 折扣

- **情境5**：單一商品購買
  - 輸入：originalPrice = 100m, quantity = 1
  - 預期結果：100m
  - 驗證重點：單一商品無折扣

**邊界條件**：

- **情境6**：折扣臨界值（10個）
  - 輸入：originalPrice = 10m, quantity = 10
  - 預期結果：95m
  - 驗證重點：正好達到 5% 折扣門檻

- **情境7**：折扣臨界值（50個）
  - 輸入：originalPrice = 10m, quantity = 50
  - 預期結果：450m
  - 驗證重點：正好達到 10% 折扣門檻

- **情境8**：折扣臨界值（100個）
  - 輸入：originalPrice = 10m, quantity = 100
  - 預期結果：850m
  - 驗證重點：正好達到 15% 折扣門檻

- **情境9**：小數價格批量購買
  - 輸入：originalPrice = 99.99m, quantity = 25
  - 預期結果：2374.7625m
  - 驗證重點：小數價格的批量折扣計算精度

- **情境10**：極大數量購買
  - 輸入：originalPrice = 1m, quantity = 10000
  - 預期結果：8500m
  - 驗證重點：大數量計算穩定性

**異常處理**：

- **情境11**：負數原始價格
  - 輸入：originalPrice = -10m, quantity = 5
  - 預期結果：拋出 ArgumentException
  - 驗證重點：負數價格驗證和錯誤訊息

- **情境12**：零數量
  - 輸入：originalPrice = 10m, quantity = 0
  - 預期結果：拋出 ArgumentException
  - 驗證重點：零數量驗證和錯誤訊息

- **情境13**：負數量
  - 輸入：originalPrice = 10m, quantity = -5
  - 預期結果：拋出 ArgumentException
  - 驗證重點：負數量驗證和錯誤訊息

- **情境14**：零價格正常數量
  - 輸入：originalPrice = 0m, quantity = 10
  - 預期結果：0m
  - 驗證重點：零價格邊界處理

### 4. 依賴關係識別

**內部方法依賴**：此方法依賴同類別內的 `CalculateDiscountedPrice` 方法和 `GetBulkDiscountRate` 私有方法進行計算，無外部依賴項目需要 Mock。