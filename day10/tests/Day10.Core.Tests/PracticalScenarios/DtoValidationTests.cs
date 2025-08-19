namespace Day10.Core.Tests.PracticalScenarios;

/// <summary>
/// DTO 驗證測試
/// </summary>
public class DtoValidationTests : AutoFixtureTestBase
{
    [Fact]
    public void ValidateCustomerRequest_有效資料_應通過驗證()
    {
        // Arrange
        var fixture = this.CreateFixture();

        // 客製化以符合驗證規則
        var request = fixture.Build<CreateCustomerRequest>()
                             .With(x => x.Name, () =>
                             {
                                 var name = fixture.Create<string>();
                                 return name.Length > 50 ? name[..50] : name;
                             })
                             .With(x => x.Email, fixture.Create<MailAddress>().Address)
                             .With(x => x.Age, Random.Shared.Next(18, 78)) // 18-77 歲，符合 [Range(18, 120)] 驗證
                             .With(x => x.Phone, "0912-345-678")           // 提供有效的電話號碼格式
                             .Create();

        // Act
        var context = new ValidationContext(request);
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(request, context, results, true);

        // Assert
        isValid.Should().BeTrue();
        results.Should().BeEmpty();
    }

    [Fact]
    public void ValidateCustomerRequest_姓名過長_應驗證失敗()
    {
        // Arrange
        var fixture = new Fixture();
        var request = fixture.Build<CreateCustomerRequest>()
                             .With(x => x.Name, new string('A', 101)) // 超過 100 字元
                             .With(x => x.Email, fixture.Create<MailAddress>().Address)
                             .With(x => x.Age, 25)
                             .Create();

        // Act
        var context = new ValidationContext(request);
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(request, context, results, true);

        // Assert
        isValid.Should().BeFalse();
        results.Should().ContainSingle(r => r.MemberNames.Contains(nameof(request.Name)));
    }

    [Fact]
    public void ValidateCustomerRequest_無效Email_應驗證失敗()
    {
        // Arrange
        var fixture = new Fixture();
        var request = fixture.Build<CreateCustomerRequest>()
                             .With(x => x.Name, "Valid Name")
                             .With(x => x.Email, "invalid-email") // 無效的電子郵件格式
                             .With(x => x.Age, 25)
                             .Create();

        // Act
        var context = new ValidationContext(request);
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(request, context, results, true);

        // Assert
        isValid.Should().BeFalse();
        results.Should().ContainSingle(r => r.MemberNames.Contains(nameof(request.Email)));
    }

    [Fact]
    public void ValidateCustomerRequest_年齡過小_應驗證失敗()
    {
        // Arrange
        var fixture = new Fixture();
        var request = fixture.Build<CreateCustomerRequest>()
                             .With(x => x.Name, "Valid Name")
                             .With(x => x.Email, fixture.Create<MailAddress>().Address)
                             .With(x => x.Age, 15) // 小於 18 歲
                             .Create();

        // Act
        var context = new ValidationContext(request);
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(request, context, results, true);

        // Assert
        isValid.Should().BeFalse();
        results.Should().ContainSingle(r => r.MemberNames.Contains(nameof(request.Age)));
    }
}