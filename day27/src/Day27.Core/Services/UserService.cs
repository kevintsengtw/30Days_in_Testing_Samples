using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Day27.Core.Interfaces;
using Day27.Core.Models;

namespace Day27.Core.Services;

/// <summary>
/// 使用者服務
/// </summary>
public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly TimeProvider _timeProvider;

    public UserService(IUserRepository userRepository, IEmailService emailService, TimeProvider timeProvider)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
    }

    /// <summary>
    /// 註冊新使用者
    /// </summary>
    /// <param name="request">註冊請求</param>
    /// <returns>註冊後的使用者</returns>
    /// <exception cref="ArgumentNullException">當請求為 null</exception>
    /// <exception cref="ValidationException">當驗證失敗</exception>
    /// <exception cref="InvalidOperationException">當 Email 已存在</exception>
    public async Task<User> RegisterUserAsync(RegisterUserRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        // 驗證 Email 格式
        if (!IsValidEmail(request.Email))
        {
            throw new ValidationException("Email 格式無效");
        }

        // 驗證密碼強度
        ValidatePassword(request.Password);

        // 檢查 Email 是否已存在
        if (await _userRepository.ExistsByEmailAsync(request.Email))
        {
            throw new InvalidOperationException("此 Email 已被註冊");
        }

        // 建立新使用者
        var user = new User
        {
            Email = request.Email,
            Name = request.Name,
            PasswordHash = HashPassword(request.Password),
            Status = UserStatus.Active,
            CreatedAt = _timeProvider.GetUtcNow().DateTime,
            UpdatedAt = _timeProvider.GetUtcNow().DateTime
        };

        // 儲存使用者
        var savedUser = await _userRepository.SaveAsync(user);

        // 發送歡迎郵件
        await _emailService.SendWelcomeEmailAsync(savedUser.Email, savedUser.Name);

        return savedUser;
    }

    /// <summary>
    /// 驗證密碼強度
    /// </summary>
    /// <param name="password">密碼</param>
    /// <exception cref="ValidationException">當密碼不符合要求</exception>
    public void ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ValidationException("密碼不能為空");
        }

        if (password.Length < 8)
        {
            throw new ValidationException("密碼長度至少 8 字元");
        }

        if (password.Length > 50)
        {
            throw new ValidationException("密碼長度不能超過 50 字元");
        }

        if (!Regex.IsMatch(password, @"[a-zA-Z]"))
        {
            throw new ValidationException("密碼必須包含字母");
        }

        if (!Regex.IsMatch(password, @"[0-9]"))
        {
            throw new ValidationException("密碼必須包含數字");
        }
    }

    /// <summary>
    /// 驗證 Email 格式
    /// </summary>
    /// <param name="email">Email 地址</param>
    /// <returns>是否為有效格式</returns>
    public bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        try
        {
            // 基本格式驗證
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
            if (!emailRegex.IsMatch(email))
            {
                return false;
            }

            // 分割 email 取得網域部分
            var parts = email.Split('@');
            if (parts.Length != 2)
            {
                return false;
            }

            var domain = parts[1];

            // 檢查網域不能以點號開始
            if (domain.StartsWith("."))
            {
                return false;
            }

            // 檢查網域不能以點號結束
            if (domain.EndsWith("."))
            {
                return false;
            }

            // 檢查網域不能包含連續點號
            if (domain.Contains(".."))
            {
                return false;
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 更新使用者狀態
    /// </summary>
    /// <param name="userId">使用者識別碼</param>
    /// <param name="newStatus">新狀態</param>
    /// <returns>更新後的使用者</returns>
    /// <exception cref="ArgumentException">當使用者不存在</exception>
    public async Task<User> UpdateUserStatusAsync(int userId, UserStatus newStatus)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new ArgumentException("使用者不存在", nameof(userId));
        }

        user.Status = newStatus;
        user.UpdatedAt = _timeProvider.GetUtcNow().DateTime;

        return await _userRepository.SaveAsync(user);
    }

    /// <summary>
    /// 產生密碼雜湊值
    /// </summary>
    /// <param name="password">原始密碼</param>
    /// <returns>雜湊值</returns>
    private static string HashPassword(string password)
    {
        var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}