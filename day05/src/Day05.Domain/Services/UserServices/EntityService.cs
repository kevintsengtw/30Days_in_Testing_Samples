using Day05.Domain.DomainModels;
using Day05.Domain.Models;

namespace Day05.Domain.Services.UserServices;

/// <summary>
/// class EntityService - 實體服務
/// </summary>
public class EntityService
{
    /// <summary>
    /// 更新用戶
    /// </summary>
    /// <param name="id">用戶ID</param>
    /// <param name="request">更新請求</param>
    /// <returns>更新後的用戶實體</returns>
    public UserEntity UpdateUser(int id, UpdateUserRequest request)
    {
        return new UserEntity
        {
            Id = id,
            Name = request.Name,
            Email = request.Email,
            CreatedAt = DateTime.Now.AddDays(-1),
            UpdatedAt = DateTime.Now,
            Version = 2,
            LastModifiedBy = "system"
        };
    }
}