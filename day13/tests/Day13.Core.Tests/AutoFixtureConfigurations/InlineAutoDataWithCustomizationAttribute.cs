using System;
using AutoFixture.Xunit2;

namespace Day13.Core.Tests.AutoFixtureConfigurations;

/// <summary>
/// class InlineAutoDataWithCustomizationAttribute
/// </summary>
public class InlineAutoDataWithCustomizationAttribute : InlineAutoDataAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InlineAutoDataWithCustomizationAttribute"/> class
    /// </summary>
    /// <param name="values">The values</param>
    public InlineAutoDataWithCustomizationAttribute(params object[] values)
        : base(new AutoDataWithCustomizationAttribute(), values)
    {
    }
}