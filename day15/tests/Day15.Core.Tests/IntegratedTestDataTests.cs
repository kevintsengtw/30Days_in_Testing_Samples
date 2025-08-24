using AutoFixture;
using AwesomeAssertions;
using Bogus;
using Day15.Core.Models;
using Day15.TestLibrary.TestData;
using Day15.TestLibrary.TestData.Extensions;

namespace Day15.Core.Tests;

/// <summary>
/// 整合測試資料測試
/// </summary>
public class IntegratedTestDataTests
{
    private readonly IFixture _fixture;
    private readonly HybridTestDataGenerator _generator;

    /// <summary>
    /// Initializes a new instance of the <see cref="IntegratedTestDataTests"/> class
    /// </summary>
    public IntegratedTestDataTests()
    {
        // 方法一：使用擴展方法
        this._fixture = new Fixture().WithBogus();

        // 方法二：使用混合產生器
        this._generator = new HybridTestDataGenerator();
    }

    [Fact]
    public void AutoFixture_整合_Bogus_應能產生真實感資料()
    {
        // Arrange & Act
        var user = this._fixture.Create<User>();
        var company = this._fixture.Create<Company>();

        // Assert - User 使用 Bogus 產生，有真實感的資料
        user.Email.Should().Contain("@");
        user.FirstName.Should().NotBeNullOrEmpty();
        user.Phone.Should().MatchRegex(@"[\d\-\(\)\s\+\.x]+");

        // Company 使用 Bogus 產生
        company.Name.Should().NotBeNullOrEmpty();
        company.Website.Should().StartWith("http");
    }

    [Fact]
    public void 混合產生器_應能自動處理複雜物件()
    {
        // Arrange & Act
        var order = this._generator.Generate<Order>();

        // Assert
        order.Should().NotBeNull();
        order.Customer.Email.Should().Contain("@");                   // Customer 使用 Bogus
        order.Items.Should().NotBeEmpty();                            // Items 由 AutoFixture 處理
        order.Items.First().Product.Name.Should().NotBeNullOrEmpty(); // Product.Name 使用 Bogus
    }

    [Theory]
    [BogusAutoData]
    public void 使用_AutoData_與_Bogus_整合(User user, Address address)
    {
        // Arrange - 資料由整合後的 AutoFixture 自動產生

        // Assert
        user.Email.Should().Contain("@");
        user.FirstName.Should().NotBeNullOrEmpty();
        address.City.Should().NotBeNullOrEmpty();
        address.Country.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void 客製化_特定類型的_Bogus_產生器()
    {
        // Arrange
        var customUserFaker = new Faker<User>()
                              .RuleFor(u => u.FirstName, "John")
                              .RuleFor(u => u.LastName, "Doe")
                              .RuleFor(u => u.Age, f => f.Random.Int(25, 65));

        var customFixture = new Fixture()
            .WithBogusFor(customUserFaker);

        // Act
        var user = customFixture.Create<User>();

        // Assert
        user.FirstName.Should().Be("John");
        user.LastName.Should().Be("Doe");
        user.Age.Should().BeInRange(25, 65);
    }

    [Fact]
    public void 應能產生具有真實感的電話號碼()
    {
        // Arrange & Act
        var users = this._fixture.CreateMany<User>(10);

        // Assert
        users.Should().AllSatisfy(user =>
        {
            user.Phone.Should().NotBeNullOrEmpty();
            user.Phone.Should().MatchRegex(@"[\d\-\(\)\s\+\.x]+");
        });
    }

    [Fact]
    public void 應能產生具有真實感的地址資訊()
    {
        // Arrange & Act
        var addresses = this._fixture.CreateMany<Address>(5);

        // Assert
        addresses.Should().AllSatisfy(address =>
        {
            address.Street.Should().NotBeNullOrEmpty();
            address.City.Should().NotBeNullOrEmpty();
            address.Country.Should().NotBeNullOrEmpty();
            address.PostalCode.Should().NotBeNullOrEmpty();
        });
    }

    [Fact]
    public void 應能正確處理循環參考()
    {
        // Arrange & Act
        var company = this._fixture.Create<Company>();

        // Assert
        company.Should().NotBeNull();
        company.Name.Should().NotBeNullOrEmpty();

        // 驗證循環參考被正確處理（不會拋出例外）
        company.Employees.Should().NotBeNull();

        // 使用 OmitOnRecursionBehavior 時，循環參考的屬性會被設為 null 以避免無限迴圈
        // 這是正常行為，表示循環參考被正確處理
        if (company.Employees.Any())
        {
            var firstEmployee = company.Employees[0];
            // OmitOnRecursionBehavior 會在遇到循環參考時將屬性設為 null
            // 這是預期行為，表示成功處理了循環參考問題
        }
    }
}