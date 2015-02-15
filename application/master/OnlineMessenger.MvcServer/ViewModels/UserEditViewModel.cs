using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using OnlineMessenger.Domain.Entities;

namespace OnlineMessenger.MvcServer.ViewModels
{
    public class UserEditViewModel
    {

        public UserEditViewModel()
        {
            Roles = new List<RoleViewModel>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string CreatedDate { get; set; }
        public List<RoleViewModel> Roles { get; set; }
        public bool IsChangePassword { get; set; }
        public int MessagesCount { get; set; }
        public string Password { get; set; }

        #region MappingSettings

        public static void InitMap()
        {
            Mapper.CreateMap<User, UserEditViewModel>()
               .ForMember(dest => dest.Name, opt => opt.MapFrom(x => x.Name))
               .ForMember(dest => dest.Roles, opt => opt.MapFrom(x => x.Roles.Select(r => new Role { Id = r.Id, Name = r.Name })))
               .ForMember(dest => dest.Email, opt => opt.MapFrom(x => x.Email))
               .ForMember(dest => dest.MessagesCount, opt => opt.MapFrom(x => x.FromMessages.Count + x.ToMessages.Count))
               .ForMember(dest => dest.Password, opt => opt.Ignore())
               .ForMember(dest => dest.Id, opt => opt.MapFrom(x => x.Id));

            Mapper.CreateMap<UserEditViewModel, User>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(x => x.Name))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(x => x.Email))
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.FromMessages, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(x => x.Roles.Select(r => new Role { Id = r.Id, Name = r.Name })))
                .ForMember(dest => dest.ToMessages, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.MapFrom(x => x.Id));
        }

        #endregion MappingSettings

    }






}