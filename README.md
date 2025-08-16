# 2025 iThome 鐵人賽 - 重啟挑戰：老派軟體工程師的測試修練

這個 Repository 包含 2025 iThome 鐵人賽系列文章的範例程式碼。

**系列文章**: <https://ithelp.ithome.com.tw/users/20066083/ironman/8276>

## 專案列表

| Day | 主題                                        | 文章連結                                               | 程式碼   |
| :-- | :------------------------------------------ | :----------------------------------------------------- | :------- |
| 01  | 老派工程師的測試啟蒙 - 為什麼我們需要測試？ | [連結](https://ithelp.ithome.com.tw/articles/10373888) | `day01/` |
| 02  | xUnit 框架深度解析 - 從生態概觀到實戰專案   | [連結](https://ithelp.ithome.com.tw/articles/10373952) | `day02/` |
| 03  | xUnit 進階功能與測試資料管理                | [連結](https://ithelp.ithome.com.tw/articles/10374064) | `day03/` |
| 04  | AwesomeAssertions 基礎應用與實戰技巧        | [連結](https://ithelp.ithome.com.tw/articles/10374188) | `day04/` |
| 05  | AwesomeAssertions 進階技巧與複雜情境應用    | [連結](https://ithelp.ithome.com.tw/articles/10374425) | `day05/` |
| 06  | Code Coverage 程式碼涵蓋範圍實戰指南        | [連結](https://ithelp.ithome.com.tw/articles/10374467) | -        |
| 07  | 依賴替代入門 - 使用 NSubstitute             | [連結](https://ithelp.ithome.com.tw/articles/10374593) | `day07/` |

## 環境需求

- .NET 9.0 SDK
- 支援 C# 的 IDE

## 執行方式

進入對應的 day 資料夾，執行：

```bash
dotnet clean    # 清除
dotnet build    # 建置
dotnet test     # 執行測試
```
