using Day14.Demo;

Console.WriteLine("=== Bogus 與 AutoBogus 範例示範 ===\n");

// 基本 Bogus 範例
Demo.DemonstrateBogusBasics();

// 台灣本土化範例
Demo.DemonstrateTaiwanData();

// 複雜物件關係範例
Demo.DemonstrateComplexObjects();

// AutoBogus 範例
Demo.DemonstrateAutoBogus();

// 效能比較範例
Demo.DemonstratePerformanceComparison();

// 可重現性範例
Demo.DemonstrateReproducibility();

Console.WriteLine("\n按任意鍵結束...");
Console.ReadKey();