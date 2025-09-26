using App.DTO;
using App.Models;
using AutoMapper;

namespace App.Mapper
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<ProductAddDTO, Produit>()
            .ForMember(dest => dest.NomProduit, opt => opt.MapFrom(src => src.Nom));

        }
    }

}
