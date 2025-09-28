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
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Nom));

        }
    }

}
