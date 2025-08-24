using System.Reflection;
using AutoFixture.Kernel;
using Bogus;

namespace Day15.TestLibrary.TestData.SpecimenBuilders;

/// <summary>
/// 網站 URL 屬性的 Bogus 整合
/// </summary>
public class WebsiteSpecimenBuilder : ISpecimenBuilder
{
    private readonly Faker _faker = new();

    /// <summary>
    /// 根據屬性名稱產生對應的網站 URL
    /// </summary>
    /// <param name="request">request</param>
    /// <param name="context">context</param>
    /// <returns></returns>
    public object Create(object request, ISpecimenContext context)
    {
        if (request is PropertyInfo property &&
            property.Name.Contains("Website", StringComparison.OrdinalIgnoreCase))
        {
            return this._faker.Internet.Url();
        }

        return new NoSpecimen();
    }
}