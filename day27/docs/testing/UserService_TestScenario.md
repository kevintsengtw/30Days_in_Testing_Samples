# UserService 測試情境分析

## RegisterUserAsync 方法

### 1. 方法簽名分析

- **參數類型**: `RegisterUserRequest request`
- **回傳類型**: `Task<User>`
- **是否為非同步**: 是

### 2. 業務邏輯分析

- **輸入驗證**:
  - 請求物件不能為 null
  - Email 格式驗證（使用正規表示式 `^[^@\s]+@[^@\s]+\.[^@\s]+$`）
  - 密碼強度驗證：不能為空、長度 8-50 字元、必須包含字母和數字
- **業務規則檢查**:
  - Email 唯一性檢查（透過 Repository 查詢）
- **資料處理**:
  - 建立使用者物件（包含密碼雜湊、狀態設定為 Active、時間戳記）
  - 儲存使用者資料
  - 發送歡迎郵件
- **異常處理**:
  - 參數為 null 時拋出 ArgumentNullException
  - 驗證失敗時拋出 ValidationException
  - Email 已存在時拋出 InvalidOperationException

### 3. 測試情境建議

**正常情境**:

- **情境1**: 有效註冊資料
  - 輸入: `{ Email: "test@example.com", Name: "張三", Password: "Password123" }`
  - 預期結果: 成功註冊並回傳使用者物件
  - 驗證重點: 密碼已雜湊、狀態為 Active、時間戳記正確設定、呼叫儲存和發送郵件

- **情境2**: 複雜 Email 格式
  - 輸入: `{ Email: "user.name+tag@example.co.uk", Name: "李四", Password: "SecurePass456" }`
  - 預期結果: 成功註冊
  - 驗證重點: 複雜但有效的 Email 格式處理

- **情境3**: 特殊字元姓名
  - 輸入: `{ Email: "test@domain.com", Name: "王小明-陳", Password: "MyPass789" }`
  - 預期結果: 成功註冊
  - 驗證重點: 包含特殊字元的姓名處理

**邊界條件**:

- **情境4**: 最短密碼長度
  - 輸入: `{ Email: "test@example.com", Name: "張三", Password: "Pass123a" }`
  - 預期結果: 成功註冊
  - 驗證重點: 8 字元密碼邊界值處理

- **情境5**: 最長密碼長度
  - 輸入: `{ Email: "test@example.com", Name: "張三", Password: "Pass123a" + "b".Repeat(42) }`
  - 預期結果: 成功註冊
  - 驗證重點: 50 字元密碼邊界值處理

- **情境6**: 最短有效 Email
  - 輸入: `{ Email: "a@b.c", Name: "張三", Password: "Password123" }`
  - 預期結果: 成功註冊
  - 驗證重點: 最短有效 Email 格式處理

**異常處理**:

- **情境7**: Null 請求物件
  - 輸入: `null`
  - 預期結果: 拋出 ArgumentNullException
  - 驗證重點: Null 參數驗證和錯誤訊息

- **情境8**: 無效 Email 格式
  - 輸入: `{ Email: "invalid-email", Name: "張三", Password: "Password123" }`
  - 預期結果: 拋出 ValidationException "Email 格式無效"
  - 驗證重點: Email 格式驗證和錯誤訊息

- **情境9**: 空白 Email
  - 輸入: `{ Email: "", Name: "張三", Password: "Password123" }`
  - 預期結果: 拋出 ValidationException "Email 格式無效"
  - 驗證重點: 空白 Email 驗證

- **情境10**: 密碼為 null 或空白
  - 輸入: `{ Email: "test@example.com", Name: "張三", Password: "" }`
  - 預期結果: 拋出 ValidationException "密碼不能為空"
  - 驗證重點: 空密碼驗證和錯誤訊息

- **情境11**: 密碼長度不足
  - 輸入: `{ Email: "test@example.com", Name: "張三", Password: "Pass12" }`
  - 預期結果: 拋出 ValidationException "密碼長度至少 8 字元"
  - 驗證重點: 密碼長度下限驗證

- **情境12**: 密碼長度過長
  - 輸入: `{ Email: "test@example.com", Name: "張三", Password: "Pass123" + "a".Repeat(45) }`
  - 預期結果: 拋出 ValidationException "密碼長度不能超過 50 字元"
  - 驗證重點: 密碼長度上限驗證

- **情境13**: 密碼缺少字母
  - 輸入: `{ Email: "test@example.com", Name: "張三", Password: "12345678" }`
  - 預期結果: 拋出 ValidationException "密碼必須包含字母"
  - 驗證重點: 密碼字母要求驗證

- **情境14**: 密碼缺少數字
  - 輸入: `{ Email: "test@example.com", Name: "張三", Password: "Password" }`
  - 預期結果: 拋出 ValidationException "密碼必須包含數字"
  - 驗證重點: 密碼數字要求驗證

- **情境15**: Email 已存在
  - 輸入: `{ Email: "existing@example.com", Name: "張三", Password: "Password123" }`
  - 預期結果: 拋出 InvalidOperationException "此 Email 已被註冊"
  - 驗證重點: Email 唯一性檢查和錯誤訊息

- **情境16**: Repository 儲存失敗
  - 輸入: 有效註冊資料
  - 預期結果: 拋出 Repository 相關異常
  - 驗證重點: 儲存失敗時的異常處理

- **情境17**: EmailService 發送失敗
  - 輸入: 有效註冊資料
  - 預期結果: 使用者已註冊但郵件發送可能失敗
  - 驗證重點: 郵件發送失敗不影響註冊流程

### 4. 依賴關係識別

- **IUserRepository**: 需要 Mock，用於 Email 存在性檢查和使用者資料儲存
  - `ExistsByEmailAsync(string email)`: 回傳 bool
  - `SaveAsync(User user)`: 回傳 User
- **IEmailService**: 需要 Mock，用於發送歡迎郵件
  - `SendWelcomeEmailAsync(string email, string userName)`: 回傳 bool

---

## ValidatePassword 方法

### 1. 方法簽名分析

- **參數類型**: `string password`
- **回傳類型**: `void`
- **是否為非同步**: 否

### 2. 業務邏輯分析

- **輸入驗證**:
  - 密碼不能為 null 或空白字串
  - 密碼長度必須在 8-50 字元之間
  - 密碼必須包含至少一個字母 (使用正規表示式 `[a-zA-Z]`)
  - 密碼必須包含至少一個數字 (使用正規表示式 `[0-9]`)
- **驗證順序**:
  1. 檢查 null 或空白
  2. 檢查最小長度 (8 字元)
  3. 檢查最大長度 (50 字元)
  4. 檢查是否包含字母
  5. 檢查是否包含數字
- **異常處理**:
  - 每個驗證失敗都拋出 ValidationException 並包含具體的錯誤訊息

### 3. 測試情境建議

**正常情境**:

- **情境1**: 有效密碼 - 包含字母和數字
  - 輸入: `"Password123"`
  - 預期結果: 不拋出任何異常
  - 驗證重點: 基本的密碼強度驗證通過

- **情境2**: 有效密碼 - 混合大小寫字母和數字
  - 輸入: `"MySecure123"`
  - 預期結果: 不拋出任何異常
  - 驗證重點: 大小寫字母都被認可為有效字母

- **情境3**: 有效密碼 - 包含特殊字元
  - 輸入: `"Test123!@#"`
  - 預期結果: 不拋出任何異常
  - 驗證重點: 特殊字元不影響驗證結果

**邊界條件**:

- **情境4**: 最短有效密碼長度
  - 輸入: `"Test123a"`
  - 預期結果: 不拋出任何異常
  - 驗證重點: 8 字元邊界值處理

- **情境5**: 最長有效密碼長度
  - 輸入: `"Test123" + "a".Repeat(43)` (總共 50 字元)
  - 預期結果: 不拋出任何異常
  - 驗證重點: 50 字元邊界值處理

- **情境6**: 最少字母數字組合
  - 輸入: `"a1bcdefg"`
  - 預期結果: 不拋出任何異常
  - 驗證重點: 最少字母和數字要求滿足

**異常處理**:

- **情境7**: Null 密碼
  - 輸入: `null`
  - 預期結果: 拋出 ValidationException "密碼不能為空"
  - 驗證重點: Null 輸入驗證和錯誤訊息

- **情境8**: 空字串密碼
  - 輸入: `""`
  - 預期結果: 拋出 ValidationException "密碼不能為空"
  - 驗證重點: 空字串驗證和錯誤訊息

- **情境9**: 空白字串密碼
  - 輸入: `"   "`
  - 預期結果: 拋出 ValidationException "密碼不能為空"
  - 驗證重點: 空白字串驗證

- **情境10**: 密碼長度不足
  - 輸入: `"Test12"`
  - 預期結果: 拋出 ValidationException "密碼長度至少 8 字元"
  - 驗證重點: 最小長度限制驗證

- **情境11**: 密碼長度過長
  - 輸入: `"Test123" + "a".Repeat(45)` (總共 52 字元)
  - 預期結果: 拋出 ValidationException "密碼長度不能超過 50 字元"
  - 驗證重點: 最大長度限制驗證

- **情境12**: 密碼缺少字母
  - 輸入: `"12345678"`
  - 預期結果: 拋出 ValidationException "密碼必須包含字母"
  - 驗證重點: 字母要求驗證

- **情境13**: 密碼缺少數字
  - 輸入: `"abcdefgh"`
  - 預期結果: 拋出 ValidationException "密碼必須包含數字"
  - 驗證重點: 數字要求驗證

- **情境14**: 密碼只包含特殊字元
  - 輸入: `"!@#$%^&*"`
  - 預期結果: 拋出 ValidationException "密碼必須包含字母"
  - 驗證重點: 特殊字元不滿足字母要求

- **情境15**: 臨界長度且缺少要求
  - 輸入: `"1234567"` (7 字元)
  - 預期結果: 拋出 ValidationException "密碼長度至少 8 字元"
  - 驗證重點: 長度驗證優先於內容驗證

### 4. 依賴關係識別

**無外部依賴**: 此方法為純函數，僅依賴輸入參數和 .NET 內建的字串操作及正規表示式功能，不需要 Mock 任何外部服務或依賴項目。

---

## IsValidEmail 方法

### 1. 方法簽名分析

- **參數類型**: `string email`
- **回傳類型**: `bool`
- **是否為非同步**: 否

### 2. 業務邏輯分析

- **輸入驗證**:
  - 檢查 email 是否為 null、空字串或只包含空白字元
- **格式驗證**:
  - 使用正規表示式 `^[^@\s]+@[^@\s]+\.[^@\s]+$` 進行格式驗證
  - 使用 IgnoreCase 選項進行不區分大小寫的比對
- **異常處理**:
  - 使用 try-catch 包裝正規表示式操作，任何異常都回傳 false
- **回傳邏輯**:
  - 輸入無效時回傳 false
  - 格式符合正規表示式時回傳 true
  - 發生任何異常時回傳 false

### 3. 測試情境建議

**正常情境**:

- **情境1**: 標準 Email 格式
  - 輸入: `"test@example.com"`
  - 預期結果: `true`
  - 驗證重點: 基本的 Email 格式驗證

- **情境2**: 包含子網域的 Email
  - 輸入: `"user@mail.example.com"`
  - 預期結果: `true`
  - 驗證重點: 子網域格式處理

- **情境3**: 包含點號的用戶名
  - 輸入: `"first.last@example.com"`
  - 預期結果: `true`
  - 驗證重點: 用戶名包含點號的格式

- **情境4**: 包含加號的用戶名
  - 輸入: `"user+tag@example.com"`
  - 預期結果: `true`
  - 驗證重點: 用戶名包含加號的格式

- **情境5**: 混合大小寫
  - 輸入: `"Test@Example.COM"`
  - 預期結果: `true`
  - 驗證重點: 大小寫不敏感驗證

**邊界條件**:

- **情境6**: 最短有效 Email
  - 輸入: `"a@b.c"`
  - 預期結果: `true`
  - 驗證重點: 最短有效格式處理

- **情境7**: 數字用戶名和網域
  - 輸入: `"123@456.789"`
  - 預期結果: `true`
  - 驗證重點: 純數字格式處理

- **情境8**: 特殊字元用戶名
  - 輸入: `"user_name@example.com"`
  - 預期結果: `true`
  - 驗證重點: 底線等特殊字元處理

**異常處理**:

- **情境9**: Null 輸入
  - 輸入: `null`
  - 預期結果: `false`
  - 驗證重點: Null 輸入處理

- **情境10**: 空字串
  - 輸入: `""`
  - 預期結果: `false`
  - 驗證重點: 空字串處理

- **情境11**: 空白字串
  - 輸入: `"   "`
  - 預期結果: `false`
  - 驗證重點: 空白字串處理

- **情境12**: 缺少 @ 符號
  - 輸入: `"testexample.com"`
  - 預期結果: `false`
  - 驗證重點: @ 符號必要性驗證

- **情境13**: 缺少網域部分
  - 輸入: `"test@"`
  - 預期結果: `false`
  - 驗證重點: 網域部分必要性驗證

- **情境14**: 缺少用戶名部分
  - 輸入: `"@example.com"`
  - 預期結果: `false`
  - 驗證重點: 用戶名部分必要性驗證

- **情境15**: 缺少頂級網域
  - 輸入: `"test@example"`
  - 預期結果: `false`
  - 驗證重點: 頂級網域必要性驗證

- **情境16**: 多個 @ 符號
  - 輸入: `"test@@example.com"`
  - 預期結果: `false`
  - 驗證重點: 多重 @ 符號處理

- **情境17**: 包含空白字元
  - 輸入: `"test @example.com"`
  - 預期結果: `false`
  - 驗證重點: 空白字元限制驗證

- **情境18**: 以點號開始的網域
  - 輸入: `"test@.example.com"`
  - 預期結果: `false`
  - 驗證重點: 網域格式限制驗證

- **情境19**: 以點號結束的網域
  - 輸入: `"test@example.com."`
  - 預期結果: `false`
  - 驗證重點: 網域結尾格式驗證

- **情境20**: 連續點號
  - 輸入: `"test@example..com"`
  - 預期結果: `false`
  - 驗證重點: 連續點號限制驗證

### 4. 依賴關係識別

**無外部依賴**: 此方法為純函數，僅依賴輸入參數和 .NET 內建的字串操作及正規表示式功能，不需要 Mock 任何外部服務或依賴項目。

---

## UpdateUserStatusAsync 方法

### 1. 方法簽名分析

- **參數類型**: `int userId`, `UserStatus newStatus`
- **回傳類型**: `Task<User>`
- **是否為非同步**: 是

### 2. 業務邏輯分析

- **輸入驗證**:
  - 使用者識別碼必須對應到現有使用者
- **資料查詢**:
  - 透過 Repository 根據 userId 查詢使用者
- **業務規則檢查**:
  - 檢查使用者是否存在
- **資料處理**:
  - 更新使用者狀態為新的狀態值
  - 更新 UpdatedAt 時間戳記為當前 UTC 時間
  - 儲存更新後的使用者資料
- **異常處理**:
  - 當使用者不存在時拋出 ArgumentException

### 3. 測試情境建議

**正常情境**:

- **情境1**: 更新使用者為 Active 狀態
  - 輸入: `userId: 1, newStatus: UserStatus.Active`
  - 預期結果: 成功更新並回傳使用者物件，狀態為 Active
  - 驗證重點: 狀態正確更新、UpdatedAt 時間戳記更新、呼叫儲存方法

- **情境2**: 更新使用者為 Inactive 狀態
  - 輸入: `userId: 2, newStatus: UserStatus.Inactive`
  - 預期結果: 成功更新並回傳使用者物件，狀態為 Inactive
  - 驗證重點: 狀態正確更新為 Inactive

- **情境3**: 更新使用者為 Suspended 狀態
  - 輸入: `userId: 3, newStatus: UserStatus.Suspended`
  - 預期結果: 成功更新並回傳使用者物件，狀態為 Suspended
  - 驗證重點: 狀態正確更新為 Suspended

**邊界條件**:

- **情境4**: 更新狀態為相同值
  - 輸入: `userId: 1, newStatus: UserStatus.Active`（使用者原本就是 Active）
  - 預期結果: 仍然成功更新並回傳使用者物件
  - 驗證重點: 即使狀態相同仍會更新 UpdatedAt 時間戳記

- **情境5**: 使用最小有效的使用者 ID
  - 輸入: `userId: 1, newStatus: UserStatus.Active`
  - 預期結果: 成功更新
  - 驗證重點: 最小 ID 值的處理

- **情境6**: 使用大數值的使用者 ID
  - 輸入: `userId: 999999, newStatus: UserStatus.Active`
  - 預期結果: 成功更新（假設使用者存在）
  - 驗證重點: 大 ID 值的處理

**異常處理**:

- **情境7**: 使用者不存在
  - 輸入: `userId: 999, newStatus: UserStatus.Active`
  - 預期結果: 拋出 ArgumentException "使用者不存在"
  - 驗證重點: 不存在使用者的錯誤處理和錯誤訊息

- **情境8**: 使用者 ID 為 0
  - 輸入: `userId: 0, newStatus: UserStatus.Active`
  - 預期結果: 拋出 ArgumentException "使用者不存在"
  - 驗證重點: 無效 ID 值的處理

- **情境9**: 使用者 ID 為負數
  - 輸入: `userId: -1, newStatus: UserStatus.Active`
  - 預期結果: 拋出 ArgumentException "使用者不存在"
  - 驗證重點: 負數 ID 值的處理

- **情境10**: Repository 查詢失敗
  - 輸入: `userId: 1, newStatus: UserStatus.Active`
  - 預期結果: 拋出 Repository 相關異常
  - 驗證重點: Repository 查詢異常的處理

- **情境11**: Repository 儲存失敗
  - 輸入: `userId: 1, newStatus: UserStatus.Active`（使用者存在但儲存失敗）
  - 預期結果: 拋出 Repository 儲存相關異常
  - 驗證重點: 儲存失敗時的異常處理

- **情境12**: 無效的 UserStatus 枚舉值
  - 輸入: `userId: 1, newStatus: (UserStatus)999`
  - 預期結果: 成功更新（.NET 允許無效枚舉值）
  - 驗證重點: 無效枚舉值的處理行為

### 4. 依賴關係識別

- **IUserRepository**: 需要 Mock，用於使用者查詢和資料儲存
  - `GetByIdAsync(int userId)`: 回傳 User 或 null
  - `SaveAsync(User user)`: 回傳 User
