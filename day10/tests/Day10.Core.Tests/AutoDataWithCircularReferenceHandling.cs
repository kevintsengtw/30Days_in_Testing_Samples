namespace Day10.Core.Tests;

/// <summary>
/// 自定義的 AutoData 屬性，可處理循環參考問題
/// </summary>
public class AutoDataWithCircularReferenceHandlingAttribute() : AutoDataAttribute(CreateFixtureWithCircularReferenceHandling)
{
    private static Fixture CreateFixtureWithCircularReferenceHandling()
    {
        var fixture = new Fixture();

        // 移除預設的循環參考拋出行為
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
               .ForEach(b => fixture.Behaviors.Remove(b));

        // 加入忽略循環參考的行為
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        // 客製化一些容易出問題的型別
        fixture.Customize<Customer>(c => c.Without(x => x.Orders)); // 避免 Customer-Order 循環參考

        fixture.Customize<Order>(c => c.Without(x => x.Customer)); // 避免 Order-Customer 循環參考

        return fixture;
    }
}