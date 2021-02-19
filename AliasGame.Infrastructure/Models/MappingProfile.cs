using AliasGame.Domain.Models;
using AutoMapper;

namespace AliasGame.Infrastructure.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<EfExpression, Expression>()
                .ForMember("Id", opt => opt.MapFrom(src => src.Id))
                .ForMember("Text", opt => opt.MapFrom(src => src.Text));
            
            CreateMap<Expression, EfExpression>()
                .ForMember("Id", opt => opt.MapFrom(src => src.Id))
                .ForMember("Text", opt => opt.MapFrom(src => src.Text));

            CreateMap<EfSession, Session>()
                .ForMember("Id", opt => opt.MapFrom(src => src.Id))
                .ForMember("FirstPlayerId", opt => opt.MapFrom(src => src.FirstPlayerId))
                .ForMember("SecondPlayerId", opt => opt.MapFrom(src => src.SecondPlayerId))
                .ForMember("ThirdPlayerId", opt => opt.MapFrom(src => src.ThirdPlayerId))
                .ForMember("FourthPlayerId", opt => opt.MapFrom(src => src.FourthPlayerId));
            
            CreateMap<Session, EfSession>()
                .ForMember("Id", opt => opt.MapFrom(src => src.Id))
                .ForMember("FirstPlayerId", opt => opt.MapFrom(src => src.FirstPlayerId))
                .ForMember("SecondPlayerId", opt => opt.MapFrom(src => src.SecondPlayerId))
                .ForMember("ThirdPlayerId", opt => opt.MapFrom(src => src.ThirdPlayerId))
                .ForMember("FourthPlayerId", opt => opt.MapFrom(src => src.FourthPlayerId));

            CreateMap<EfUser, User>()
                .ForMember("Id", opt => opt.MapFrom(src => src.Id))
                .ForMember("Username", opt => opt.MapFrom(src => src.UserName))
                .ForMember("Email", opt => opt.MapFrom(src => src.Email))
                .ForMember("EmailConfirmed", opt => opt.MapFrom(src => src.EmailConfirmed))
                .ForMember("PasswordHash", opt => opt.MapFrom(src => src.PasswordHash))
                .ForMember("TotalGames", opt => opt.MapFrom(src => src.TotalGames))
                .ForMember("Wins", opt => opt.MapFrom(src => src.Wins));
        }
    }
}