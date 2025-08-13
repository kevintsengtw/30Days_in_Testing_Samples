using Day04.Domain.Services;

namespace Day04.Domain.Tests.BasicAssertions;

/// <summary>
/// class ExceptionAssertionTests - 例外斷言測試
/// </summary>
public class ExceptionAssertionTests
{
    [Fact]
    public void Throw_無效參數_應拋出ArgumentException()
    {
        var userService = new UserService();

        // 基本例外斷言
        Action action = () => userService.GetUser(-1);

        action.Should().Throw<ArgumentException>()
              .WithMessage("使用者 ID 必須為正數*")
              .And.ParamName.Should().Be("id");
    }

    [Fact]
    public void NotThrow_有效操作_應不拋出例外()
    {
        var calculator = new Calculator();

        // 確保不拋出例外
        Action action = () => calculator.Add(1, 2);

        action.Should().NotThrow();
    }

    [Fact]
    public void Throw_特定例外類型檢查_應匹配預期例外()
    {
        var service = new ValidationService();

        // 驗證特定例外類型
        Action action = () => service.ValidateEmail("");

        action.Should().Throw<ArgumentException>()
              .WithMessage("*email*");
    }

    [Fact]
    public void Divide_除以零_應拋出DivideByZeroException()
    {
        var calculator = new Calculator();

        Action action = () => calculator.Divide(10, 0);

        action.Should().Throw<DivideByZeroException>()
              .WithMessage("除數不能為零");
    }
}