using AutoMapper;
using Web_API.Models;
using Web_API.Models.ViewModels;

namespace Web_API.Helper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductSummaryVM>();
        }
    }
}