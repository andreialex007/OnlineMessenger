using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using OnlineMessenger.Domain.Entities;

namespace OnlineMessenger.MvcServer.ViewModels
{
    public class UserTableItemViewModel
    {
        public UserTableItemViewModel()
        {
            Roles = new List<string>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string CreatedDate { get; set; }
        public List<string> Roles { get; set; }
        public int MessagesCount { get; set; }


        #region MappingSettings

        public static void InitMap()
        {
            Mapper.CreateMap<User, UserTableItemViewModel>()
               .ForMember(dest => dest.Name, opt => opt.MapFrom(x => x.Name))
               .ForMember(dest => dest.Roles, opt => opt.MapFrom(x => x.Roles.Select(r => r.Name).ToList()))
               .ForMember(dest => dest.Email, opt => opt.MapFrom(x => x.Email))
               .ForMember(dest => dest.MessagesCount, opt => opt.MapFrom(x => x.FromMessages.Count + x.ToMessages.Count))
               .ForMember(dest => dest.Id, opt => opt.MapFrom(x => x.Id));
        }

        #endregion MappingSettings
    }
}