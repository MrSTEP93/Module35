using AutoMapper;
using Microsoft.Extensions.Hosting;
using System;
using System.Net;
using UnsocNetwork.Models;
using UnsocNetwork.ViewModels.Account;

namespace UnsocNetwork
{
    public class MappingProfile : Profile
    {
        /// <summary>
        /// В конструкторе настроим соответствие сущностей при маппинге
        /// </summary>
        public MappingProfile()
        {
            CreateMap<RegisterViewModel, User>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.EmailReg))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.EmailReg))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => new DateTime(src.Year, src.Month, src.Date)));

            CreateMap<LoginViewModel, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
        }
    }
}
