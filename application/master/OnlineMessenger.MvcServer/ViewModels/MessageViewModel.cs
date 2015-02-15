using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using OnlineMessenger.Domain.Entities;

namespace OnlineMessenger.MvcServer.ViewModels
{
    public class MessageViewModel
    {
        public MessageViewModel()
        {
            Date = DateTime.Now;
            To = new List<string>();
        }

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
        public int FromId { get; set; }
        public virtual User From { get; set; }
        public virtual List<string> To { get; set; }

        #region MappingSettings

        public static void InitMap()
        {
            Mapper.CreateMap<Message, MessageViewModel>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(x => x.Date))
                .ForMember(dest => dest.From, opt => opt.MapFrom(x => x.From))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(x => x.Text))
                .ForMember(dest => dest.To, opt => opt.MapFrom(x => x.To.Select(s => s.Name).ToList()))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(x => x.Id));
        }

        #endregion MappingSettings
    }
}