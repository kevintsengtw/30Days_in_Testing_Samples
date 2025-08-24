using System.Reflection;
using AutoFixture.Kernel;
using Bogus;

namespace Day15.TestLibrary.TestData.SpecimenBuilders;

/// <summary>
/// 姓名屬性的 Bogus 整合
/// </summary>
public class NameSpecimenBuilder : ISpecimenBuilder
{
    private readonly Faker _faker = new();

    /// <summary>
    /// 根據屬性名稱產生對應的姓名資料
    /// </summary>
    /// <param name="request">request</param>
    /// <param name="context">context</param>
    /// <returns></returns>
    public object Create(object request, ISpecimenContext context)
    {
        if (request is PropertyInfo property)
        {
            return property.Name.ToLower() switch
            {
                var name when name.Contains("firstname") => this._faker.Person.FirstName,
                var name when name.Contains("lastname") => this._faker.Person.LastName,
                var name when name.Contains("fullname") => this._faker.Person.FullName,
                _ => new NoSpecimen()
            };
        }
        return new NoSpecimen();
    }
}

