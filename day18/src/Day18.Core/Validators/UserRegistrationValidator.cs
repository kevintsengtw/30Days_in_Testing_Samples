using System.Text.RegularExpressions;
using Day18.Core.Models;
using FluentValidation;

namespace Day18.Core.Validators;

/// <summary>
/// class UserRegistrationValidator - 使用者註冊請求驗證器
/// </summary>
public class UserRegistrationValidator : AbstractValidator<UserRegistrationRequest>
{
    private readonly TimeProvider _timeProvider;

    public UserRegistrationValidator() : this(TimeProvider.System)
    {
    }

    public UserRegistrationValidator(TimeProvider timeProvider)
    {
        this._timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        this.SetupValidationRules();
    }

    /// <summary>
    /// 設定所有驗證規則
    /// </summary>
    private void SetupValidationRules()
    {
        // 使用者名稱驗證
        this.RuleFor(x => x.Username)
            .NotEmpty().WithMessage("使用者名稱不可為 null 或空白")
            .Length(3, 20).WithMessage("使用者名稱長度必須在 3 到 20 個字元之間")
            .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("使用者名稱只能包含字母、數字和底線");

        // 電子郵件驗證
        this.RuleFor(x => x.Email)
            .NotEmpty().WithMessage("電子郵件不可為 null 或空白")
            .EmailAddress().WithMessage("電子郵件格式不正確")
            .MaximumLength(100).WithMessage("電子郵件長度不能超過 100 個字元");

        // 密碼驗證
        this.RuleFor(x => x.Password)
            .NotEmpty().WithMessage("密碼不可為 null 或空白")
            .Length(8, 50).WithMessage("密碼長度必須在 8 到 50 個字元之間")
            .Must(this.BeComplexPassword).WithMessage("密碼必須包含大小寫字母和數字");

        // 確認密碼驗證
        this.RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("確認密碼必須與密碼相同");

        // 年齡驗證
        this.RuleFor(x => x.Age)
            .GreaterThanOrEqualTo(18).WithMessage("年齡必須大於或等於 18 歲")
            .LessThanOrEqualTo(120).WithMessage("年齡必須小於或等於 120 歲");

        // 複雜的跨欄位驗證
        this.RuleFor(x => x.BirthDate)
            .Must((request, birthDate) => this.IsAgeConsistentWithBirthDate(birthDate, request.Age))
            .WithMessage("生日與年齡不一致");

        // 條件式驗證
        this.RuleFor(x => x.PhoneNumber)
            .Matches(@"^09\d{8}$").WithMessage("電話號碼格式不正確")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

        // 角色驗證
        this.RuleFor(x => x.Roles)
            .NotEmpty().WithMessage("至少需要指定一個角色")
            .Must(roles => roles == null || roles.All(role => this.IsValidRole(role)))
            .WithMessage("包含無效的角色");

        // 同意條款驗證
        this.RuleFor(x => x.AgreeToTerms)
            .Equal(true).WithMessage("必須同意使用條款");
    }

    private bool BeComplexPassword(string password)
    {
        return !string.IsNullOrEmpty(password) &&
               Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$");
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