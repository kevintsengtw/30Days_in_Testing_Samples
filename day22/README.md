# Day 22 – 範例專案

> [Day 22 - Testcontainers 整合測試：MongoDB 及 Redis 基礎到進階](https://ithelp.ithome.com.tw/articles/10376524)

---

## 專案結構

```text
day22/
├── Day22.Samples.sln                        # 方案檔
├── .gitignore                               # 忽略設定
├── src/
│   └── Day22.Core/
│       ├── Day22.Core.csproj                # 核心專案 csproj
│       ├── GlobalUsings.cs                  # 全域 using 設定
│       ├── Configuration/
│       │   ├── MongoDbSettings.cs           # MongoDB 連線設定
│       │   └── RedisSettings.cs             # Redis 連線設定
│       ├── Infrastructure/
│       │   ├── MongoDbConfig.cs             # MongoDB 組態物件
│       │   └── RedisConfig.cs               # Redis 組態物件
│       ├── Extensions/
│       │   └── DateTimeExtensions.cs        # DateTime 擴充方法
│       ├── Models/
│       │   ├── Mongo/
│       │   │   ├── UserDocument.cs          # MongoDB 使用者文件
│       │   │   ├── UserProfile.cs           # 使用者個人資料
│       │   │   ├── Skill.cs                 # 技能模型
│       │   │   ├── SkillLevel.cs            # 技能等級列舉
│       │   │   ├── GeoLocation.cs           # 地理位置模型
│       │   │   └── Address.cs               # 地址模型
│       │   └── Redis/
│       │       ├── UserSession.cs           # 使用者會話資料
│       │       ├── RecentView.cs            # 最近檢視項目
│       │       ├── NotificationMessage.cs   # 通知訊息
│       │       ├── LeaderboardEntry.cs      # 排行榜項目
│       │       └── CacheItem.cs             # 快取項目包裝器
│       └── Services/
│           ├── RedisCacheService.cs         # Redis 快取服務
│           ├── MongoUserService.cs          # MongoDB 使用者服務
│           ├── IUserService.cs              # 使用者服務介面
│           └── ICacheService.cs             # 快取服務介面
├── tests/
│   └── Day22.Integration.Tests/
│       ├── Day22.Integration.Tests.csproj   # 測試專案 csproj
│       ├── GlobalUsings.cs                  # 測試專案全域 using 設定
│       ├── Fixtures/
│       │   ├── MongoDbContainerFixture.cs   # Testcontainers MongoDB 容器管理
│       │   ├── RedisContainerFixture.cs     # Testcontainers Redis 容器管理
│       │   └── TestSettings.cs              # 測試用設定產生器
│       ├── MongoDB/
│       │   ├── MongoUserServiceTests.cs     # MongoDB 使用者服務整合測試
│       │   ├── MongoIndexTests.cs           # MongoDB 索引效能測試
│       │   └── MongoBsonTests.cs            # MongoDB BSON 處理測試
│       ├── Redis/
│       │   └── RedisCacheServiceTests.cs    # Redis 快取服務整合測試
│       └── Extensions/
│           └── DateTimeExtensionsTests.cs   # DateTime 擴充方法測試
```

---

## 專案內容簡介

### 1. `src/Day22.Core/`
- **Day22.Core.csproj**：專案描述與套件依賴，支援 MongoDB、Redis、DI、Logging 等。
- **GlobalUsings.cs**：全域 using 設定，簡化程式碼。
- **Configuration/MongoDbSettings.cs**：MongoDB 連線與集合設定。
- **Configuration/RedisSettings.cs**：Redis 連線與快取設定。
- **Infrastructure/MongoDbConfig.cs**：MongoDB 組態物件，管理連線字串與資料庫名稱。
- **Infrastructure/RedisConfig.cs**：Redis 組態物件，管理連線字串與資料庫編號。
- **Extensions/DateTimeExtensions.cs**：DateTime 擴充方法，提供年齡計算等功能。
- **Models/Mongo/**：MongoDB 相關資料模型
	- **UserDocument.cs**：使用者主文件，含個人資料、技能、地址等。
	- **UserProfile.cs**：使用者個人資料，巢狀文件範例。
	- **Skill.cs**：技能模型，支援技能等級與認證。
	- **SkillLevel.cs**：技能等級列舉。
	- **GeoLocation.cs**：地理位置模型，支援 GeoJSON 格式。
	- **Address.cs**：地址模型，支援地理空間查詢。
- **Models/Redis/**：Redis 相關資料模型
	- **UserSession.cs**：使用者會話資料，Hash 範例。
	- **RecentView.cs**：最近檢視項目，List 範例。
	- **NotificationMessage.cs**：通知訊息，Stream 範例。
	- **LeaderboardEntry.cs**：排行榜項目，Sorted Set 範例。
	- **CacheItem.cs**：快取項目包裝器，支援過期與分群。
- **Services/RedisCacheService.cs**：Redis 快取服務，展示各種 Redis 操作。
- **Services/MongoUserService.cs**：MongoDB 使用者服務，展示各種 MongoDB 操作。
- **Services/IUserService.cs**：使用者服務介面，定義 CRUD 與查詢。
- **Services/ICacheService.cs**：快取服務介面，定義 Redis 操作。

### 2. `tests/Day22.Integration.Tests/`
- **Day22.Integration.Tests.csproj**：測試專案描述，引用 Testcontainers、xUnit、AwesomeAssertions 等。
- **GlobalUsings.cs**：測試專案全域 using 設定。
- **Fixtures/MongoDbContainerFixture.cs**：Testcontainers 管理 MongoDB 容器生命週期，提供測試用資料庫環境。
- **Fixtures/RedisContainerFixture.cs**：Testcontainers 管理 Redis 容器生命週期，提供測試用快取環境。
- **Fixtures/TestSettings.cs**：產生測試用設定物件、Logger、FakeTimeProvider。
- **MongoDB/MongoUserServiceTests.cs**：MongoDB 使用者服務整合測試，驗證 CRUD、查詢、索引等功能。
- **MongoDB/MongoIndexTests.cs**：MongoDB 索引效能測試，驗證唯一索引、查詢效能。
- **MongoDB/MongoBsonTests.cs**：MongoDB BSON 處理測試，驗證 ObjectId、自動序列化等。
- **Redis/RedisCacheServiceTests.cs**：Redis 快取服務整合測試，驗證 String、Hash、List、Stream、SortedSet 等操作。
- **Extensions/DateTimeExtensionsTests.cs**：DateTime 擴充方法測試，驗證年齡計算等。

---

## 測試專案各測試類別說明

- **MongoUserServiceTests.cs**：針對 MongoDB 使用者服務進行 CRUD、查詢、索引建立等整合測試。
- **MongoIndexTests.cs**：測試 MongoDB 唯一索引、複合索引與查詢效能。
- **MongoBsonTests.cs**：測試 BSON ObjectId 產生、序列化與資料一致性。
- **RedisCacheServiceTests.cs**：針對 Redis 快取服務進行各種資料結構（String、Hash、List、Stream、SortedSet）操作測試。
- **DateTimeExtensionsTests.cs**：測試 DateTime 擴充方法，包含年齡計算等。

- **Fixtures/MongoDbContainerFixture.cs**、**Fixtures/RedisContainerFixture.cs**：負責啟動/關閉 MongoDB、Redis Docker 容器，並提供測試用連線物件。
- **Fixtures/TestSettings.cs**：產生測試用設定、Logger、FakeTimeProvider，簡化測試環境建置。

---

此專案展示如何以 Testcontainers 建立 MongoDB 與 Redis 測試環境，並結合 .NET 進行整合測試與進階查詢。
