using System.Data;
using Microsoft.Data.SqlClient;

namespace Day03.Core.Repositories;

/// <summary>
/// class UserRepository - 使用者資料的儲存庫實現。
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserRepository"/> class.
    /// </summary>
    /// <param name="connectionString">資料庫連接字串。</param>
    public UserRepository(string connectionString)
    {
        this._connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    /// <summary>
    /// 獲取所有使用者的集合。
    /// </summary>
    /// <returns>所有使用者的可列舉集合。</returns>
    public IEnumerable<User> GetAllUsers()
    {
        var users = new List<User>();
        using var connection = new SqlConnection(this._connectionString);
        connection.Open();

        const string sql = "SELECT Id, Name, Email, CreatedAt FROM Users";
        using var command = new SqlCommand(sql, connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            users.Add(new User
            {
                Id = reader.GetInt32("Id"),
                Name = reader.GetString("Name"),
                Email = reader.GetString("Email"),
                CreatedAt = reader.GetDateTime("CreatedAt")
            });
        }

        return users;
    }

    /// <summary>
    /// 根據電子郵件地址檢索使用者。
    /// </summary>
    /// <param name="email">要檢索的使用者的電子郵件地址。</param>
    /// <returns>具有指定電子郵件地址的使用者，若不存在則返回 <c>null</c>。</returns>
    public User? GetUserByEmail(string email)
    {
        using var connection = new SqlConnection(this._connectionString);
        connection.Open();

        const string sql = "SELECT Id, Name, Email, CreatedAt FROM Users WHERE Email = @Email";
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Email", email);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new User
            {
                Id = reader.GetInt32("Id"),
                Name = reader.GetString("Name"),
                Email = reader.GetString("Email"),
                CreatedAt = reader.GetDateTime("CreatedAt")
            };
        }

        return null;
    }

    /// <summary>
    /// 在儲存庫中建立新使用者。
    /// </summary>
    /// <param name="user">要建立的使用者。</param>
    /// <returns>已建立的使用者。</returns>
    public User CreateUser(User user)
    {
        using var connection = new SqlConnection(this._connectionString);
        connection.Open();

        const string sql = "INSERT INTO Users (Name, Email) OUTPUT INSERTED.Id, INSERTED.CreatedAt VALUES (@Name, @Email)";
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Name", user.Name);
        command.Parameters.AddWithValue("@Email", user.Email);

        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return user;
        }

        user.Id = reader.GetInt32("Id");
        user.CreatedAt = reader.GetDateTime("CreatedAt");

        return user;
    }
}