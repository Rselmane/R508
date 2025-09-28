using App.DTO;
using App.Models;
using AutoMapper;

public class TypeProductMapper : Profile
{
    public TypeProductMapper()
    {
        CreateMap<TypeProduct, TypeProductDTO>()
        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdTypeProduct))
        .ForMember(dest => dest.Nom, opt => opt.MapFrom(src => src.TypeProductName));
    }
}
