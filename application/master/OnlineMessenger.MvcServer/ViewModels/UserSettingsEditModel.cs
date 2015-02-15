using AutoMapper;
using OnlineMessenger.Domain.Entities;

namespace OnlineMessenger.MvcServer.ViewModels
{
    public class UserSettingsEditModel
    {

        public UserSettingsEditModel()
        {
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string CreatedDate { get; set; }
        public bool IsChangePassword { get; set; }
        public int MessagesCount { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string PasswordConfirm { get; set; }
        public bool AudioNotificationsEnabled { get; set; }
        public bool VisualNotificationsEnabled { get; set; }

        #region MappingSettings

        public static void InitMap()
        {
            Mapper.CreateMap<User, UserSettingsEditModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(x => x.Name))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(x => x.Email))
                .ForMember(dest => dest.NewPassword, opt => opt.Ignore())
                .ForMember(dest => dest.CurrentPassword, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordConfirm, opt => opt.Ignore())
                .ForMember(dest => dest.AudioNotificationsEnabled, opt => opt.MapFrom(r => r.AudioNotificationsEnabled))
                .ForMember(dest => dest.VisualNotificationsEnabled, opt => opt.MapFrom(r => r.VisualNotificationsEnabled));

            Mapper.CreateMap<UserSettingsEditModel, User>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(x => x.Name))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(x => x.Email))
                .ForMember(dest => dest.AudioNotificationsEnabled, opt => opt.MapFrom(r => r.AudioNotificationsEnabled))
                .ForMember(dest => dest.VisualNotificationsEnabled, opt => opt.MapFrom(x => x.VisualNotificationsEnabled))
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.FromMessages, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.Roles, opt => opt.Ignore())
                .ForMember(dest => dest.ToMessages, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }

        #endregion MappingSettings

    }
}