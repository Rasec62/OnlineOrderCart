using AutoMapper;
using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Web.Models.Dtos;

namespace OnlineOrderCart.Web.OnlineOrderCartMappers
{
    public class OnlineOrderCartMapper : Profile
    {
        public OnlineOrderCartMapper(){
            CreateMap<Users, AddUserDto> ().ReverseMap();
        }
    }
}
