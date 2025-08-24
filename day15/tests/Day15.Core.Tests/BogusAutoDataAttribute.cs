using System;
using AutoFixture;
using AutoFixture.Xunit2;
using Day15.TestLibrary.TestData.Extensions;

namespace Day15.Core.Tests;

/// <summary>
/// 整合 Bogus 的 AutoData 屬性
/// </summary>
public class BogusAutoDataAttribute : AutoDataAttribute
{
    public BogusAutoDataAttribute() : base(() => new Fixture().WithBogus())
    {
    }
}
