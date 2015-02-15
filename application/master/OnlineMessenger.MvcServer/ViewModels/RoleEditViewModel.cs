using AutoMapper;
using OnlineMessenger.Domain.Entities;

namespace OnlineMessenger.MvcServer.ViewModels
{
    public class RoleEditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        #region MappingSettings

        public static void InitMap()
        {
            Mapper.CreateMap<Role, RoleEditViewModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(x => x.Name))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(x => x.Id));

            Mapper.CreateMap<RoleEditViewModel, Role>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(x => x.Name))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(x => x.Id));
        }

        #endregion MappingSettings
    }
}