using System;

namespace Day03.Core.Tests.BuilderPatternTests;

/// <summary>
/// class UserBuilder - 用於建構使用者物件的建造者類別。
/// </summary>
public class UserBuilder
{
    private string _name = "Default User";
    private string _email = "default@example.com";
    private int _age = 25;
    private readonly List<string> _roles = [];
    private UserSettings _settings = new();

    /// <summary>
    /// 設定使用者名稱
    /// </summary>
    public UserBuilder WithName(string name)
    {
        this._name = name;
        return this;
    }

    /// <summary>
    /// 設定使用者電子郵件
    /// </summary>
    public UserBuilder WithEmail(string email)
    {
        this._email = email;
        return this;
    }

    /// <summary>
    /// 設定使用者年齡
    /// </summary>
    public UserBuilder WithAge(int age)
    {
        this._age = age;
        return this;
    }

    /// <summary>
    /// 設定使用者角色
    /// </summary>
    public UserBuilder WithRole(string role)
    {
        this._roles.Add(role);
        return this;
    }

    /// <summary>
    /// 設定使用者角色
    /// </summary>
    public UserBuilder WithRoles(params string[] roles)
    {
        this._roles.AddRange(roles);
        return this;
    }

    /// <summary>
    /// 設定使用者管理權限
    /// </summary>
    public UserBuilder WithAdminRights()
    {
        return this.WithRoles("Admin", "User");
    }

    public UserBuilder WithSettings(UserSettings settings)
    {
        this._settings = settings;
        return this;
    }

    /// <summary>
    /// 建立使用者物件
    /// </summary>
    public User Build()
    {
        return new User
        {
            Name = this._name,
            Email = this._email,
            Age = this._age,
            Roles = this._roles.ToArray(),
            Settings = this._settings
        };
    }

    /// <summary>
    /// 建立一般使用者物件 (預設建立者方法)
    /// </summary>
    public static UserBuilder AUser()
    {
        return new UserBuilder();
    }

    /// <summary>
    /// 建立管理員使用者物件
    /// </summary>
    public static UserBuilder AnAdminUser()
    {
        return new UserBuilder().WithAdminRights();
    }

    /// <summary>
    /// 建立一般使用者物件
    /// </summary>
    public static UserBuilder ARegularUser()
    {
        return new UserBuilder().WithRole("User");
    }
}