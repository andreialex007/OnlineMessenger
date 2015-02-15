using AutoMapper;
using OnlineMessenger.Domain.Entities;

namespace OnlineMessenger.MvcServer.ViewModels
{
    public class RoleTableItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UsersCount { get; set; }

        #region MappingSettings

        public static void InitMap()
        {
            Mapper.CreateMap<Role, RoleTableItemViewModel>()
               .ForMember(dest => dest.Name, opt => opt.MapFrom(x => x.Name))
               .ForMember(dest => dest.UsersCount, opt => opt.MapFrom(x => x.Users.Count))
               .ForMember(dest => dest.Id, opt => opt.MapFrom(x => x.Id));
        }

        #endregion MappingSettings
    }
}