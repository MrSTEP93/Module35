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
            //    .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.EmailReg))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => new DateTime(src.Year, src.Month, src.Day)));

            CreateMap<LoginViewModel, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

            CreateMap<User, UserEditViewModel>()
                .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.BirthDate.Year))
                .ForMember(dest => dest.Month, opt => opt.MapFrom(src => src.BirthDate.Month))
                .ForMember(dest => dest.Day, opt => opt.MapFrom(src => src.BirthDate.Day));
                        
            CreateMap<UserEditViewModel,User>()
                //.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => new DateTime(src.Year, src.Month, src.Day)));

            CreateMap<User, UserWithFriendExt>();
        }
    }
}
