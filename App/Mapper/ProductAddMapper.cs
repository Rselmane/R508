using App.DTO;
using App.Models;
using AutoMapper;

namespace App.Mapper
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<ProductAddDTO, Product>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.ActualStock, opt => opt.MapFrom(src => src.Stock));

        }
    }

}
