namespace Day10.Core.Services;

/// <summary>
/// 使用者服務
/// </summary>
public class UserService
{
    /// <summary>
    /// 建立使用者
    /// </summary>
    public User CreateUser(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        return new User
        {
            Id = Random.Shared.Next(1, 10000),
            Name = user.Name,
            Email = user.Email,
            Age = user.Age
        };
    }
}