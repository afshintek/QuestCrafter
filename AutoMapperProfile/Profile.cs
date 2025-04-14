using AutoMapper;
using QuestCrafter.DTO;
using QuestCrafter.Models;

namespace QuestCrafter.AutoMapperProfile
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateUserDTO, User>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.username))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.email))
            // will be ignored automatically since these properties don't share the same name, therefore there'll be no auto-matching
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()); // no need for reversemap() cuz we don't need to create a "createUserDTO from a user!


            CreateMap<QuestDTO, Quest>(); // names in both model match

            CreateMap<User, User>()
            .ForMember(dest => dest.CreatedQuests, opt => opt.Ignore())
            .ForMember(dest => dest.Participations, opt => opt.Ignore())
            .ForMember(dest => dest.LeaderBoardEntries, opt => opt.Ignore());

            CreateMap<Participant, Participant>()
            .ForMember(dest => dest.Quest, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());

            CreateMap<LeaderBoard, LeaderBoard>()
            .ForMember(dest => dest.Quest, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());

            
        CreateMap<Quest, Quest>();

        }
    }
}