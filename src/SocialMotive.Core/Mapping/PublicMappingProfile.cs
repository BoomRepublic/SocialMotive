using AutoMapper;
using SocialMotive.Core.Data;
using SocialMotive.Core.Model.Public;

namespace SocialMotive.Core.Mapping
{
    /// <summary>
    /// AutoMapper profile for Public area entity → DTO mappings
    /// </summary>
    public class PublicMappingProfile : Profile
    {
        public PublicMappingProfile()
        {
            #region Event

            CreateMap<DbEvent, Event>()
                .ForMember(d => d.EventTypeName, opt => opt.MapFrom(s => s.EventType != null ? s.EventType.Name : string.Empty))
                .ForMember(d => d.EventTypeIcon, opt => opt.MapFrom(s => s.EventType != null ? s.EventType.Icon : null))
                .ForMember(d => d.EventTypeColor, opt => opt.MapFrom(s => s.EventType != null ? s.EventType.ColorHex : null))
                .ForMember(d => d.OrganizerName, opt => opt.MapFrom(s => s.Organizer != null ? (s.Organizer.FirstName + " " + s.Organizer.LastName).Trim() : string.Empty))
                .ForMember(d => d.ParticipantCount, opt => opt.MapFrom(s => s.Participants.Count));

            #endregion

            #region EventDetail

            CreateMap<DbEvent, EventDetail>()
                .ForMember(d => d.EventTypeName, opt => opt.MapFrom(s => s.EventType != null ? s.EventType.Name : string.Empty))
                .ForMember(d => d.EventTypeIcon, opt => opt.MapFrom(s => s.EventType != null ? s.EventType.Icon : null))
                .ForMember(d => d.EventTypeColor, opt => opt.MapFrom(s => s.EventType != null ? s.EventType.ColorHex : null))
                .ForMember(d => d.OrganizerName, opt => opt.MapFrom(s => s.Organizer != null ? (s.Organizer.FirstName + " " + s.Organizer.LastName).Trim() : string.Empty))
                .ForMember(d => d.OrganizerBio, opt => opt.MapFrom(s => s.Organizer != null ? s.Organizer.Bio : null))
                .ForMember(d => d.ParticipantCount, opt => opt.MapFrom(s => s.Participants.Count))
                .ForMember(d => d.Tasks, opt => opt.MapFrom(s => s.EventTasks.OrderBy(t => t.OrderIndex)));

            #endregion

            #region EventTask

            CreateMap<DbEventTask, EventTask>();

            #endregion

            #region EventType

            CreateMap<DbEventType, EventType>();

            #endregion

            #region City

            CreateMap<DbCity, City>();

            #endregion
        }
    }
}
