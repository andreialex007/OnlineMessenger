using System;
using AutoMapper;
using OnlineMessenger.Domain.Entities;

namespace OnlineMessenger.MvcServer.ViewModels
{
    public class MessageDisplayViewModel
    {
        public MessageDisplayViewModel()
        {
        }

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
        public string From { get; set; }

        #region MappingSettings

        public static void InitMap()
        {
            Mapper.CreateMap<Message, MessageDisplayViewModel>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(x => x.Date))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(x => x.Text))
                .ForMember(dest => dest.From, opt => opt.MapFrom(x => x.From.Name))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(x => x.Id));
        }

        #endregion MappingSettings
    }
}