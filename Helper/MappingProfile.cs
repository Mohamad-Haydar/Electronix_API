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
            CreateMap<Manufacturer, ProductSummaryVM>();
            CreateMap<Category, ProductSummaryVM>();

            CreateMap<Product, ProductSummaryVM>().IncludeMembers(src => src.Category, m => m.Manufacturer);


            CreateMap<User, RegisterVM>();
            CreateMap<User, DashboardUsersVM>();
            CreateMap<DashbordUser, DashboardUsersVM>().IncludeMembers(src => src.User);
            CreateMap<User, ClientVM>();
        }
    }
}