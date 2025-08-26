# Day 17 – 範例專案

> [Day 17 – 檔案與 IO 測試：使用 System.IO.Abstractions 模擬檔案系統 - 實現可測試的檔案操作](https://ithelp.ithome.com.tw/articles/10375981)

---

## 專案結構

```
day17/
├── Day17.Samples.sln
├── src/
│   └── Day17.Core/
│       ├── ConfigManagerService.cs
│       ├── ConfigurationService.cs
│       ├── Day17.Core.csproj
│       ├── FileManagerService.cs
│       ├── FilePermissionService.cs
│       ├── GlobalUsings.cs
│       └── StreamProcessorService.cs
└── tests/
    └── Day17.Core.Tests/
        ├── ConfigManagerServiceTests.cs
        ├── ConfigurationServiceTests.cs
        ├── Day17.Core.Tests.csproj
        ├── FileManagerServiceTests.cs
        ├── FilePermissionServiceTests.cs
        ├── GlobalUsings.cs
        └── StreamProcessorServiceTests.cs
```

---

## 專案內容說明

### src/Day17.Core
- **ConfigManagerService.cs**：整合多種檔案與設定管理，模擬實際應用場景的設定檔管理服務。
- **ConfigurationService.cs**：負責設定檔案的讀取與寫入，支援預設值與例外處理。
- **FileManagerService.cs**：檔案管理服務，提供檔案複製、刪除、移動等常用操作。
- **FilePermissionService.cs**：檔案權限檢查服務，判斷檔案是否可讀、可寫。
- **StreamProcessorService.cs**：檔案串流處理服務，支援文字檔案逐行轉換、二進位串流處理等。
- **GlobalUsings.cs**：全域 using 設定。
- **Day17.Core.csproj**：主專案組態檔。

### tests/Day17.Core.Tests
- **ConfigManagerServiceTests.cs**：ConfigManagerService 的單元測試，驗證設定目錄初始化、設定檔 CRUD 等情境。
- **ConfigurationServiceTests.cs**：ConfigurationService 的單元測試，驗證設定檔案的讀寫、預設值、例外處理。
- **FileManagerServiceTests.cs**：FileManagerService 的單元測試，驗證檔案複製、刪除、移動等操作。
- **FilePermissionServiceTests.cs**：FilePermissionService 的單元測試，驗證檔案權限檢查（可讀/可寫/不存在等）。
- **StreamProcessorServiceTests.cs**：StreamProcessorService 的單元測試，驗證文字檔案轉換、串流處理等。
- **GlobalUsings.cs**：測試專案全域 using 設定。
- **Day17.Core.Tests.csproj**：測試專案組態檔。

---

## 技術重點

- **System.IO.Abstractions 注入**：所有檔案操作皆透過 IFileSystem 介面，方便替換與測試。
- **MockFileSystem 單元測試**：測試時不需實際存取磁碟，提升測試速度與安全性。
- **多元檔案情境**：涵蓋設定檔、權限、複製、串流等多種常見檔案操作。
- **真實應用導向**：每個服務與測試皆對應實際開發常見需求。

---

本專案示範如何利用 System.IO.Abstractions 來抽換 .NET FileSystem，讓檔案操作邏輯可被單元測試。內容包含：

- 以 IFileSystem 介面取代直接操作 System.IO，實現檔案系統注入
- 如何設計可測試的檔案、目錄、權限、串流等服務
- 使用 MockFileSystem 進行單元測試，不需實際存取磁碟
- 多種常見檔案操作情境的測試實作
