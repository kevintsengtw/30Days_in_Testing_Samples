namespace Day14.Core.Extensions;

/// <summary>
/// 台灣資料擴充
/// </summary>
public static class TaiwanDataExtensions
{
    // 只包含直轄市和省轄市
    public static readonly string[] Cities =
    [
        "台北市", "新北市", "桃園市", "台中市", "台南市", "高雄市",
        "新竹市", "嘉義市", "基隆市"
    ];

    // 所有縣市（包含縣和市）
    public static readonly string[] AllAreas =
    [
        "台北市", "新北市", "桃園市", "台中市", "台南市", "高雄市",
        "基隆市", "新竹市", "嘉義市", "宜蘭縣", "新竹縣", "苗栗縣",
        "彰化縣", "南投縣", "雲林縣", "嘉義縣", "屏東縣", "台東縣",
        "花蓮縣", "澎湖縣", "金門縣", "連江縣"
    ];

    // 知名大學和科技大學
    public static readonly string[] Universities =
    [
        "台灣大學", "清華大學", "交通大學", "成功大學", "中山大學",
        "政治大學", "中央大學", "中興大學", "師範大學", "東華大學",
        "台北科技大學", "雲林科技大學", "高雄科技大學", "中正大學"
    ];

    // 知名企業和簡稱
    public static readonly string[] Companies =
    [
        "台積電", "鴻海", "聯發科", "廣達", "仁寶", "和碩", "華碩", "宏碁",
        "中華電信", "台灣大哥大", "遠傳電信", "統一企業", "台塑", "中鋼",
        "台達電", "日月光", "矽品", "國泰金控", "富邦金控", "中信金控",
        "緯創", "英業達", "統一", "國泰", "中信", "遠傳", "富邦"
    ];

    /// <summary>
    /// 產生台灣城市（只包含直轄市）
    /// </summary>
    public static string TaiwanCity(this Faker faker)
    {
        return faker.PickRandom(Cities);
    }

    /// <summary>
    /// 產生台灣縣市（包含所有縣市）
    /// </summary>
    public static string TaiwanArea(this Faker faker)
    {
        return faker.PickRandom(AllAreas);
    }

    /// <summary>
    /// 產生台灣的大學
    /// </summary>
    public static string TaiwanUniversity(this Faker faker)
    {
        return faker.PickRandom(Universities);
    }

    /// <summary>
    /// 產生台灣的公司
    /// </summary>
    public static string TaiwanCompany(this Faker faker)
    {
        return faker.PickRandom(Companies);
    }

    /// <summary>
    /// 產生台灣身分證字號（僅供測試）
    /// </summary>
    public static string TaiwanIdCard(this Faker faker)
    {
        var firstChar = faker.PickRandom("ABCDEFGHJKLMNPQRSTUVXYWZIO");
        var genderDigit = faker.Random.Int(1, 2);
        var digits = faker.Random.String2(8, "0123456789");
        return $"{firstChar}{genderDigit}{digits}";
    }

    /// <summary>
    /// 產生台灣手機號碼
    /// </summary>
    public static string TaiwanMobilePhone(this Faker faker)
    {
        const string prefix = "09";
        var middle = faker.Random.Int(0, 9);
        var suffix = faker.Random.String2(7, "0123456789");
        return $"{prefix}{middle}{suffix}";
    }
}