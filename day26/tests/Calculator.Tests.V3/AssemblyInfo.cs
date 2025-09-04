using Calculator.Tests.V3.Fixtures;

// 啟用 Console 和 Trace 輸出捕獲
[assembly: CaptureConsole]
[assembly: CaptureTrace]

// 註冊 Assembly Fixture
[assembly: AssemblyFixture(typeof(DatabaseAssemblyFixture))]

// 備註：xUnit v3 的文化設定方式可能有所不同，暫時移除
// [assembly: Culture("zh-TW")]