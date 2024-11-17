using AutoMapper;
using Cofidis.Credit.Api.Dto;
using Cofidis.Credit.Domain.Entities;

namespace Cofidis.Credit.Api.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<CreditRequest, CreditRequestDto>();
            CreateMap<RiskAnalysis, RiskAnalysisDto>();
        }
       
    }
}
