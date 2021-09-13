using AutoMapper;
using CommandsService.Dtos;
using CommandsService.Models;
using PlatformService.Protos;

namespace CommandsService.Profiles
{
    public class CommandsProfile : Profile
    {
        public CommandsProfile()
        {
            // Source -> Target
            CreateMap<Platform, PlatformReadDto>();
            CreateMap<CommandCreateDto, Command>();
            CreateMap<Command, CommandReadDto>();

            CreateMap<PlatformPublishedDto, Platform>()
                .ForMember(dest => dest.ExternalID, opt => opt.MapFrom(src => src.Id));

            CreateMap<GrpcPlatformModel, Platform>()
                .ForMember(dest => dest.ExternalID, opt => opt.MapFrom(src => src.PlatformId));
        }
    }
}
