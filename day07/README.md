# Day 07：範例專案

[Day 07：依賴替代入門 - 使用 NSubstitute](https://ithelp.ithome.com.tw/articles/10374593)

---

## 專案結構

```text
day07/
├── src/
│   ├── Day07.Legacy/
│   │   ├── BackupResult.cs
│   │   ├── bin/
│   │   ├── Day07.Legacy.csproj
│   │   ├── FileBackupService.cs
│   │   └── obj/
│   └── Day07.Refactored/
│       ├── Abstractions/
│       │   ├── IBackupRepository.cs
│       │   ├── IDateTimeProvider.cs
│       │   ├── IFileInfo.cs
│       │   └── IFileSystem.cs
│       ├── BackupResult.cs
│       ├── bin/
│       ├── Day07.Refactored.csproj
│       ├── FileBackupService.cs
│       ├── GlobalUsings.cs
│       ├── Implementations/
│       │   ├── DateTimeProvider.cs
│       │   ├── FileInfoWrapper.cs
│       │   ├── FileSystemWrapper.cs
│       │   └── SqlBackupRepository.cs
│       └── obj/
├── tests/
│   └── Day07.Tests/
│       ├── bin/
│       ├── Day07.Tests.csproj
│       ├── FileBackupServiceTests.cs
│       ├── GlobalUsings.cs
│       └── obj/
└── Day07.Samples.sln
```

---

## 專案內容簡介

### src/Day07.Legacy/
- **FileBackupService.cs**、**BackupResult.cs**：傳統寫法的檔案備份服務與結果模型。

### src/Day07.Refactored/
- **Abstractions/**：備份服務的抽象介面（如 IFileSystem、IFileInfo、IDateTimeProvider、IBackupRepository），方便依賴注入與替代。
- **Implementations/**：各種抽象的實作類別（如 FileSystemWrapper、DateTimeProvider、SqlBackupRepository 等）。
- **FileBackupService.cs**：重構後的檔案備份服務，支援依賴注入。

### tests/Day07.Tests/
- **FileBackupServiceTests.cs**：針對重構後的 FileBackupService 進行單元測試，示範 NSubstitute 建立替身（Stub/Mock）來隔離依賴，驗證備份流程、異常處理與各種情境。

---

此資料夾為 Day 07 的範例專案，重點在於依賴替代（Dependency Substitution）與 NSubstitute 的實務應用。