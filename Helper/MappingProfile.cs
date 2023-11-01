using AutoMapper;
using Web_API.Models;
using Web_API.Models.DTO.Request;
using Web_API.Models.DTO.Responce;

namespace Web_API.Helper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductSummaryVM>();
            CreateMap<User, RegisterVM>();
        }
    }
}