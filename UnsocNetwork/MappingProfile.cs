using AutoMapper;
using Microsoft.Extensions.Hosting;
using System;
using System.Net;
using UnsocNetwork.Models;
using UnsocNetwork.ViewModels.Account;

namespace UnsocNetwork
{

    public class Source
    {
        public int Value { get; set; }
    }

    public class Destination
    {
        public string Value { get; set; }
    }

    public class MappingProfile : Profile
    {
        /// <summary>
        /// В конструкторе настроим соответствие сущностей при маппинге
        /// </summary>
        public MappingProfile()
        {
            CreateMap<RegisterViewModel, User>()
                .ForMember("Email", dest => dest.MapFrom(src => src.EmailReg))
                .ForMember("NickName", dest => dest.MapFrom(src => src.FirstName))
                .ForMember("BirthDate", opt => opt.ConvertUsing(src => new DateTime(src.Year, src.Month, src.Date)));

            CreateMap<Source, Destination>()
        .ForMember(dest => dest.Value, opt => opt.ConvertUsing(src => src.Value.ToString()));

            //.ForMember("BirthDate", dest => dest.MapFrom(src => src.)

            // .ForMember("Name", opt => opt.MapFrom(c => c.FirstName + " " + c.LastName))
        }
    }
}
