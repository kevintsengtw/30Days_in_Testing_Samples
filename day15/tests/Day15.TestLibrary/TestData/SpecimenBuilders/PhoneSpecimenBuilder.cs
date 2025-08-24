using System.Reflection;
using AutoFixture.Kernel;
using Bogus;

namespace Day15.TestLibrary.TestData.SpecimenBuilders;

/// <summary>
/// 電話號碼屬性的 Bogus 整合
/// </summary>
public class PhoneSpecimenBuilder : ISpecimenBuilder
{
    private readonly Faker _faker = new();

    /// <summary>
    /// 根據屬性名稱產生對應的電話號碼
    /// </summary>
    /// <param name="request">request</param>
    /// <param name="context">context</param>
    /// <returns></returns>
    public object Create(object request, ISpecimenContext context)
    {
        if (request is PropertyInfo property && property.Name.Contains("Phone", StringComparison.OrdinalIgnoreCase))
        {
            return this._faker.Phone.PhoneNumber();
        }

        return new NoSpecimen();
    }
}