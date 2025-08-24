using System.Reflection;
using AutoFixture.Kernel;
using Bogus;

namespace Day15.TestLibrary.TestData.SpecimenBuilders;

/// <summary>
/// Email 屬性的 Bogus 整合
/// </summary>
public class EmailSpecimenBuilder : ISpecimenBuilder
{
    private readonly Faker _faker = new();

    /// <summary>
    /// 根據屬性名稱產生對應的 Email
    /// </summary>
    /// <param name="request">request</param>
    /// <param name="context">context</param>
    /// <returns></returns>
    public object Create(object request, ISpecimenContext context)
    {
        if (request is PropertyInfo property && property.Name.Contains("Email", StringComparison.OrdinalIgnoreCase))
        {
            return this._faker.Internet.Email();
        }
        return new NoSpecimen();
    }
}

