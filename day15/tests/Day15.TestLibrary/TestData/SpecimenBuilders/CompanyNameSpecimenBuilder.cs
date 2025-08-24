using System.Reflection;
using AutoFixture.Kernel;
using Bogus;

namespace Day15.TestLibrary.TestData.SpecimenBuilders;

/// <summary>
/// 公司名稱屬性的 Bogus 整合
/// </summary>
public class CompanyNameSpecimenBuilder : ISpecimenBuilder
{
    private readonly Faker _faker = new();

    /// <summary>
    /// 根據屬性名稱產生對應的公司名稱
    /// </summary>
    /// <param name="request">request</param>
    /// <param name="context">context</param>
    /// <returns></returns>
    public object Create(object request, ISpecimenContext context)
    {
        if (request is PropertyInfo { DeclaringType.Name: "Company" } property &&
            property.Name.Contains("Name", StringComparison.OrdinalIgnoreCase))
        {
            return this._faker.Company.CompanyName();
        }

        return new NoSpecimen();
    }
}