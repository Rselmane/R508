using App.DTO;
using App.Models;
using AutoMapper;

namespace App.Mapper;

public class BrandMapper : Profile
{
    public BrandMapper()
    {
        // Marque -> MarqueDto
        CreateMap<Brand, BrandDTO>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdBrand))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.BrandName));

        // MarqueDto -> Marque
        CreateMap<BrandDTO, Brand>()
            .ForMember(dest => dest.IdBrand, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Products, opt => opt.Ignore()); // On ignore la navigation
    }
}
