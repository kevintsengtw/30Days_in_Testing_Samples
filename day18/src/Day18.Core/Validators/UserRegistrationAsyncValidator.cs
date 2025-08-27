using Day18.Core.Models;
using Day18.Core.Services;
using FluentValidation;

namespace Day18.Core.Validators;

/// <summary>
/// class UserRegistrationAsyncValidator - 用戶註冊請求非同步驗證器
/// </summary>
public class UserRegistrationAsyncValidator : AbstractValidator<UserRegistrationRequest>
{
    private readonly IUserService _userService;
    private readonly TimeProvider _timeProvider;

    public UserRegistrationAsyncValidator(IUserService userService) : this(userService, TimeProvider.System)
    {
    }

    public UserRegistrationAsyncValidator(IUserService userService, TimeProvider timeProvider)
    {
        this._userService = userService ?? throw new ArgumentNullException(nameof(userService));
        this._timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        this.ConfigureValidationRules();
    }

    private void ConfigureValidationRules()
    {
        // 基本規則 (同步驗證)
        this.RuleFor(x => x.Username)
            .NotEmpty().WithMessage("使用者名稱不能為空")
            .Length(3, 20).WithMessage("使用者名稱長度必須在 3 到 20 個字元之間")
            .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("使用者名稱只能包含字母、數字和底線");

        this.RuleFor(x => x.Email)
            .NotEmpty().WithMessage("電子郵件不能為空")
            .EmailAddress().WithMessage("電子郵件格式不正確")
            .MaximumLength(100).WithMessage("電子郵件長度不能超過 100 個字元");

        this.RuleFor(x => x.Password)
            .NotEmpty().WithMessage("密碼不能為空")
            .Length(8, 50).WithMessage("密碼長度必須在 8 到 50 個字元之間")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$")
            .WithMessage("密碼必須包含至少一個大寫字母、一個小寫字母和一個數字");

        this.RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("確認密碼必須與密碼相同");

        this.RuleFor(x => x.Age)
            .GreaterThanOrEqualTo(18).WithMessage("年齡必須大於或等於 18 歲")
            .LessThanOrEqualTo(120).WithMessage("年齡必須小於或等於 120 歲");

        this.RuleFor(x => x.BirthDate)
            .Must((request, birthDate) => this.IsAgeConsistentWithBirthDate(birthDate, request.Age))
            .WithMessage("生日與年齡不一致");

        this.RuleFor(x => x.PhoneNumber)
            .Matches(@"^09\d{8}$").WithMessage("電話號碼格式不正確")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

        this.RuleFor(x => x.Roles)
            .NotEmpty().WithMessage("至少需要指定一個角色")
            .Must(roles => roles.All(role => this.IsValidRole(role)))
            .WithMessage("包含無效的角色");

        this.RuleFor(x => x.AgreeToTerms)
            .Equal(true).WithMessage("必須同意使用條款");

        // 非同步驗證規則
        this.RuleFor(x => x.Username)
            .MustAsync(async (username, cancellation) =>
                           await this._userService.IsUsernameAvailableAsync(username))
            .WithMessage("使用者名稱已被使用");

        this.RuleFor(x => x.Email)
            .MustAsync(async (email, cancellation) =>
                           !await this._userService.IsEmailRegisteredAsync(email))
            .WithMessage("此電子郵件已被註冊");
    }

    /// <summary>
    /// 檢查年齡是否與生日一致
    /// </summary>
    private bool IsAgeConsistentWithBirthDate(DateTime birthDate, int age)
    {
        var currentDate = this._timeProvider.GetLocalNow().Date;
        var calculatedAge = currentDate.Year - birthDate.Year;

        if (birthDate.Date > currentDate.AddYears(-calculatedAge))
        {
            calculatedAge--;
        }

        return calculatedAge == age;
    }

    /// <summary>
    /// 檢查角色是否有效
    /// </summary>
    private bool IsValidRole(string role)
    {
        var validRoles = new[] { "User", "Admin", "Manager", "Support" };
        return validRoles.Contains(role);
    }
}