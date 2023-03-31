using AutoMapper;
using Graduation.DTOs;
using Graduation.Entities;

namespace Graduation.Helpers
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<RegisterDto, AppUser>();
        }
    }
}
