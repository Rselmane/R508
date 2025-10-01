using App.DTO;
using App.Models;
using AutoMapper;

namespace App.Mapper
{
    public class BrandUpdateMapper : Profile
    {
        public BrandUpdateMapper()
        {
            // Marque -> MarqueDtoUpdate
            CreateMap<Brand, BrandUpdateDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.BrandName));

            // MarqueDto -> Marque
            CreateMap<BrandUpdateDTO, Brand>()
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.IdBrand, opt => opt.Ignore())
                .ForMember(dest => dest.Products, opt => opt.Ignore()); // On ignore la navigation
        }
    }
}
