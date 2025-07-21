using AutoMapper;
using SecurityPortal.Application.DTOs;
using SecurityPortal.Domain.Entities;

namespace SecurityPortal.Application.Mapping;

public class IncidentMappingProfile : Profile
{
    public IncidentMappingProfile()
    {
        CreateMap<SecurityIncident, IncidentDto>()
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.Level));

        CreateMap<SecurityIncident, IncidentSummaryDto>()
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.Level));

        CreateMap<IncidentComment, IncidentCommentDto>();
        
        CreateMap<IncidentAttachment, IncidentAttachmentDto>();
    }
}