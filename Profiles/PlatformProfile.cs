using AutoMapper;
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
	}
}
