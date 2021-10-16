using AutoMapper;
using BUPTReportOnline.DTOs;
using BUPTReportOnline.Models;

namespace BUPTReportOnline
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<User, UserDTO>();
        }
    }
}
