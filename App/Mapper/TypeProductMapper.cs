using App.DTO;
using App.Models;
using AutoMapper;

public class TypeProductMapper : Profile
{
    public TypeProductMapper()
    {
        CreateMap<TypeProduit, TypeProduitDTO>()
        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdTypeProduit))
        .ForMember(dest => dest.Nom, opt => opt.MapFrom(src => src.NomTypeProduit));
    }
}
