﻿using AutoMapper;
using PlatformService.DTOs;
using PlatformService.Models;

namespace PlatformService.Profiles;

public class PlatformProfile : Profile
{
    public PlatformProfile()
    {
        CreateMap<Platform, PlatformCreateDTO>();
        CreateMap<Platform, PlatformReadDTO>();

        CreateMap<PlatformReadDTO, Platform>();
        CreateMap<PlatformCreateDTO, Platform>();

        CreateMap<PlatformReadDTO, PlatformPublishedDTO>()
            .ForMember(dest => dest.Event, opt => opt.MapFrom(src => "PlatformPublished"));

        CreateMap<Platform, GrpcPlatformModel>()
            .ForMember(dest => dest.PlatformId, opt => opt.MapFrom(src => src.Id));
    }
}