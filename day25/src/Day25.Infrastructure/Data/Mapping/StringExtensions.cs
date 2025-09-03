using System.Text;

namespace Day25.Infrastructure.Data.Mapping;

/// <summary>
/// 字串擴充方法
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// 將 snake_case 轉換為 PascalCase
    /// </summary>
    public static string ToPascalCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        // 使用 StringSplitOptions.RemoveEmptyEntries 避免因連續底線產生空字串
        var parts = input.Split(new[] { '_', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        var result = new StringBuilder();

        foreach (var part in parts)
        {
            if (!string.IsNullOrEmpty(part))
            {
                result.Append(char.ToUpper(part[0]));
                if (part.Length > 1)
                {
                    result.Append(part.Substring(1).ToLower()); // 建議將其餘部分轉為小寫，更符合標準
                }
            }
        }

        return result.ToString();
    }
}