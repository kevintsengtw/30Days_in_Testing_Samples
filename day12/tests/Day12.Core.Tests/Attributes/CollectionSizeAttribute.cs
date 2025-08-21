using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;

namespace Day12.Core.Tests.Attributes;

/// <summary>
/// 控制集合大小的自訂屬性
/// </summary>
public class CollectionSizeAttribute : CustomizeAttribute
{
    private readonly int _size;

    public CollectionSizeAttribute(int size)
    {
        this._size = size;
    }

    public override ICustomization GetCustomization(ParameterInfo parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter);

        var objectType = parameter.ParameterType.GetGenericArguments()[0];

        var isTypeCompatible = parameter.ParameterType.IsGenericType &&
                               parameter.ParameterType.GetGenericTypeDefinition()
                                        .MakeGenericType(objectType)
                                        .IsAssignableFrom(typeof(List<>).MakeGenericType(objectType));

        if (!isTypeCompatible)
        {
            throw new InvalidOperationException(
                $"{nameof(CollectionSizeAttribute)} 指定的型別與 List 不相容: {parameter.ParameterType} {parameter.Name}");
        }

        var customizationType = typeof(CollectionSizeCustomization<>).MakeGenericType(objectType);
        return (ICustomization)Activator.CreateInstance(customizationType, parameter, this._size)!;
    }

    private class CollectionSizeCustomization<T> : ICustomization
    {
        private readonly ParameterInfo _parameter;
        private readonly int _repeatCount;

        public CollectionSizeCustomization(ParameterInfo parameter, int repeatCount)
        {
            this._parameter = parameter;
            this._repeatCount = repeatCount;
        }

        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(
                new FilteringSpecimenBuilder(
                    new FixedBuilder(fixture.CreateMany<T>(this._repeatCount).ToList()),
                    new EqualRequestSpecification(this._parameter)));
        }
    }
}