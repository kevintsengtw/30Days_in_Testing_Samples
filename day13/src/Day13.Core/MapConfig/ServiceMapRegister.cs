using Day13.Core.Dto;
using Day13.Core.Entities;
using Mapster;

namespace Day13.Core.MapConfig;

/// <summary>
/// class ServiceMapRegister
/// </summary>
public class ServiceMapRegister : IRegister
{
    /// <summary>
    /// 註冊對應規則
    /// </summary>
    /// <param name="config">對應設定</param>
    public void Register(TypeAdapterConfig config)
    {
        // ShipperModel 到 ShipperDto 的對應
        config.NewConfig<ShipperModel, ShipperDto>()
              .Map(dest => dest.ShipperId, src => src.ShipperId)
              .Map(dest => dest.CompanyName, src => src.CompanyName)
              .Map(dest => dest.ContactName, src => src.ContactName)
              .Map(dest => dest.Phone, src => src.Phone)
              .Map(dest => dest.Fax, src => src.Fax)
              .Map(dest => dest.Address, src => src.Address)
              .Map(dest => dest.City, src => src.City)
              .Map(dest => dest.PostalCode, src => src.PostalCode)
              .Map(dest => dest.Country, src => src.Country);

        // ShipperDto 到 ShipperModel 的對應
        config.NewConfig<ShipperDto, ShipperModel>()
              .Map(dest => dest.ShipperId, src => src.ShipperId)
              .Map(dest => dest.CompanyName, src => src.CompanyName)
              .Map(dest => dest.ContactName, src => src.ContactName)
              .Map(dest => dest.Phone, src => src.Phone)
              .Map(dest => dest.Fax, src => src.Fax)
              .Map(dest => dest.Address, src => src.Address)
              .Map(dest => dest.City, src => src.City)
              .Map(dest => dest.PostalCode, src => src.PostalCode)
              .Map(dest => dest.Country, src => src.Country);
    }
}