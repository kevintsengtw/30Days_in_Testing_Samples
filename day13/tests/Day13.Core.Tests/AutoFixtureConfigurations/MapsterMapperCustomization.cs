using System;
using AutoFixture;
using Day13.Core.MapConfig;
using Mapster;
using MapsterMapper;

namespace Day13.Core.Tests.AutoFixtureConfigurations;

/// <summary>
/// Mapster 對應器客製化
/// </summary>
public class MapsterMapperCustomization : ICustomization
{
    /// <summary>
    /// 客製化 Fixture
    /// </summary>
    /// <param name="fixture">Fixture 實例</param>
    public void Customize(IFixture fixture)
    {
        fixture.Register(() => this.Mapper);
    }

    private IMapper? _mapper;

    private IMapper Mapper
    {
        get
        {
            if (this._mapper is not null)
            {
                return this._mapper;
            }

            var typeAdapterConfig = new TypeAdapterConfig();
            typeAdapterConfig.Scan(typeof(ServiceMapRegister).Assembly);
            this._mapper = new Mapper(typeAdapterConfig);
            return this._mapper;
        }
    }
}