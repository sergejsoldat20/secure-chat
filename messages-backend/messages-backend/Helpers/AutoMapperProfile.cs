using AutoMapper;
using messages_backend.Models.DTO;
using messages_backend.Models.Entities;

namespace messages_backend.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() 
        {
            CreateMap<Account, AuthenticateResponse>();
            CreateMap<RegisterRequest, Account>();
            CreateMap<Account, AccountResponse>();
        }
    }
}
