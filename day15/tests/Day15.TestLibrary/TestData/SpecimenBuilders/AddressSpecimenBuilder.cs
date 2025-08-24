using System.Reflection;
using AutoFixture.Kernel;
using Bogus;

namespace Day15.TestLibrary.TestData.SpecimenBuilders;

/// <summary>
/// 地址屬性的 Bogus 整合
/// </summary>
public class AddressSpecimenBuilder : ISpecimenBuilder
{
    private readonly Faker _faker = new();

    /// <summary>
    /// 根據屬性名稱產生對應的地址資料
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
                var name when name.Contains("street") => this._faker.Address.StreetAddress(),
                var name when name.Contains("city") => this._faker.Address.City(),
                var name when name.Contains("postal") || name.Contains("zip") => this._faker.Address.ZipCode(),
                var name when name.Contains("country") => this._faker.Address.Country(),
                _ => new NoSpecimen()
            };
        }

        return new NoSpecimen();
    }
}