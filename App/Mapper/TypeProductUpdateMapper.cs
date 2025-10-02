using App.DTO;
using App.Models;
using AutoMapper;

namespace App.Mapper;

public class TypeProductUpdateMapper : Profile
{
    public TypeProductUpdateMapper()
    {
        // TypeProduit -> TypeProductUpdateDTO
        CreateMap<TypeProduct, TypeProductUpdateDTO>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.TypeProductName));

        // TypeProductUpdateDTO -> TypeProduit
        CreateMap<TypeProductUpdateDTO, TypeProduct>()
            .ForMember(dest => dest.IdTypeProduct, opt => opt.Ignore())
            .ForMember(dest => dest.TypeProductName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Products, opt => opt.Ignore()); // On ignore la navigation
    }
}