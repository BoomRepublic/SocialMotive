using AutoMapper;
using SocialMotive.Core.Data;
using Volunteer = SocialMotive.Core.Model.Volunteer;

namespace SocialMotive.Core.Mapping
{
    /// <summary>
    /// AutoMapper profile for Volunteer area entity ↔ DTO mappings
    /// </summary>
    public class VolunteerMappingProfile : Profile
    {
        public VolunteerMappingProfile()
        {
            #region Profile

            CreateMap<DbUser, Volunteer.Profile>();

            CreateMap<Volunteer.ProfileUpdate, DbUser>()
                .ForMember(d => d.UserId, opt => opt.Ignore())
                .ForMember(d => d.Username, opt => opt.Ignore())
                .ForMember(d => d.PasswordHash, opt => opt.Ignore())
                .ForMember(d => d.ProfileImage, opt => opt.Ignore())
                .ForMember(d => d.CoverImage, opt => opt.Ignore())
                .ForMember(d => d.Created, opt => opt.Ignore())
                .ForMember(d => d.Modified, opt => opt.Ignore())
                .ForMember(d => d.OrganizerEvents, opt => opt.Ignore())
                .ForMember(d => d.Participations, opt => opt.Ignore())
                .ForMember(d => d.TaskAssignments, opt => opt.Ignore())
                .ForMember(d => d.SocialAccounts, opt => opt.Ignore())
                .ForMember(d => d.UserGroups, opt => opt.Ignore())
                .ForMember(d => d.UserLabels, opt => opt.Ignore())
                .ForMember(d => d.UserRoles, opt => opt.Ignore());

            #endregion

            #region Participation

            CreateMap<DbEventParticipant, Volunteer.Participation>()
                .ForMember(d => d.EventTitle, opt => opt.MapFrom(s => s.Event != null ? s.Event.Title : string.Empty))
                .ForMember(d => d.EventStartDate, opt => opt.MapFrom(s => s.Event != null ? s.Event.StartDate : default(DateTime)))
                .ForMember(d => d.EventEndDate, opt => opt.MapFrom(s => s.Event != null ? s.Event.EndDate : default(DateTime)));

            #endregion

            #region TaskAssignment

            CreateMap<DbEventTaskAssignment, Volunteer.TaskAssignment>()
                .ForMember(d => d.TaskTitle, opt => opt.MapFrom(s => s.EventTask != null ? s.EventTask.Name : string.Empty))
                .ForMember(d => d.TaskDescription, opt => opt.MapFrom(s => s.EventTask != null ? s.EventTask.Description : null))
                .ForMember(d => d.EventId, opt => opt.MapFrom(s => s.EventTask != null ? s.EventTask.EventId : 0))
                .ForMember(d => d.EventTitle, opt => opt.MapFrom(s => s.EventTask != null && s.EventTask.Event != null ? s.EventTask.Event.Title : string.Empty));

            #endregion
        }
    }
}
