using System.Reflection;
using Day25.Infrastructure.Data.Mapping;

namespace Day25.Infrastructure.Data;

/// <summary>
/// Dapper 型別映射自動註冊
/// </summary>
public static class DapperTypeMapping
{
    /// <summary>
    /// 初始化 Dapper 型別映射
    /// 自動為指定命名空間下的所有實體類別註冊 snake_case 映射
    /// </summary>
    public static void Initialize()
    {
        // 取得 Domain 組件
        var domainAssembly = typeof(Product).Assembly;

        // 篩選出 Day25.Domain 命名空間下的所有實體類別
        var entityTypes = domainAssembly.GetTypes()
                                        .Where(t => t is { IsClass: true, IsAbstract: false, Namespace: "Day25.Domain" } &&
                                                    !t.Name.EndsWith("Exception")); // 排除例外類別

        foreach (var type in entityTypes)
        {
            UseSnakeCaseMapping(type);
        }
    }

    /// <summary>
    /// 為指定型別啟用 snake_case 映射
    /// </summary>
    private static void UseSnakeCaseMapping(Type entityType)
    {
        var map = new CustomPropertyTypeMap(entityType, (type, columnName) => GetPropertyInfo(type, columnName));
        SqlMapper.SetTypeMap(entityType, map);
    }

    /// <summary>
    /// 根據欄位名稱尋找對應的屬性
    /// </summary>
    private static PropertyInfo? GetPropertyInfo(Type type, string columnName)
    {
        // 先嘗試直接匹配
        var property = type.GetProperty(columnName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        if (property != null)
        {
            return property;
        }

        // 將 snake_case 轉換為 PascalCase
        var pascalCaseName = columnName.ToPascalCase();

        // 尋找對應的屬性
        return type.GetProperty(pascalCaseName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
    }
}