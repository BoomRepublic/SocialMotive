using AutoMapper;
using SocialMotive.Core.Data;
using SocialMotive.Core.Model.Admin;

namespace SocialMotive.Core.Mapping
{
    /// <summary>
    /// AutoMapper profile for Admin area entity ↔ DTO mappings
    /// </summary>
    public class AdminMappingProfile : Profile
    {
        public AdminMappingProfile()
        {
            #region User

            CreateMap<DbUser, User>()
                .ForMember(d => d.CityId, opt => opt.MapFrom(s => s.CityId));

            CreateMap<User, DbUser>()
                .ForMember(d => d.PasswordHash, opt => opt.Ignore())
                .ForMember(d => d.ProfileImage, opt => opt.Ignore())
                .ForMember(d => d.CoverImage, opt => opt.Ignore())
                .ForMember(d => d.Created, opt => opt.Ignore())
                .ForMember(d => d.Modified, opt => opt.Ignore())
                .ForMember(d => d.CityId, opt => opt.MapFrom(s => s.CityId))
                .ForMember(d => d.OrganizerEvents, opt => opt.Ignore())
                .ForMember(d => d.Participations, opt => opt.Ignore())
                .ForMember(d => d.TaskAssignments, opt => opt.Ignore())
                .ForMember(d => d.SocialAccounts, opt => opt.Ignore())
                .ForMember(d => d.UserGroups, opt => opt.Ignore())
                .ForMember(d => d.UserLabels, opt => opt.Ignore())
                .ForMember(d => d.UserRoles, opt => opt.Ignore());

            #endregion

            #region Tracker

            CreateMap<DbTracker, Tracker>();

            CreateMap<Tracker, DbTracker>()
                .ForMember(d => d.InviteCode, opt => opt.Ignore())
                .ForMember(d => d.InviteName, opt => opt.Ignore())
                .ForMember(d => d.QrGuid, opt => opt.Ignore())
                .ForMember(d => d.CheckInTime, opt => opt.Ignore())
                .ForMember(d => d.CheckInLat, opt => opt.Ignore())
                .ForMember(d => d.CheckInLon, opt => opt.Ignore())
                .ForMember(d => d.CreatedAt, opt => opt.Ignore())
                .ForMember(d => d.ModifiedAt, opt => opt.Ignore())
                .ForMember(d => d.JoinedAt, opt => opt.Ignore())
                .ForMember(d => d.InvitedBy_TrackerId, opt => opt.Ignore())
                .ForMember(d => d.InvitedByTrackerId, opt => opt.Ignore())
                .ForMember(d => d.CheckInBy_TrackerId, opt => opt.Ignore())
                .ForMember(d => d.CheckInByTrackerId, opt => opt.Ignore())
                .ForMember(d => d.Group, opt => opt.Ignore())
                .ForMember(d => d.TrackerRole, opt => opt.Ignore())
                .ForMember(d => d.City, opt => opt.Ignore())
                .ForMember(d => d.Invite, opt => opt.Ignore())
                .ForMember(d => d.Locations, opt => opt.Ignore())
                .ForMember(d => d.TrackerLabels, opt => opt.Ignore());

            CreateMap<TrackerUpdateRequest, DbTracker>()
                .ForMember(d => d.TrackerId, opt => opt.Ignore())
                .ForMember(d => d.InviteCode, opt => opt.Ignore())
                .ForMember(d => d.QrGuid, opt => opt.Ignore())
                .ForMember(d => d.JoinedAt, opt => opt.Ignore())
                .ForMember(d => d.CreatedAt, opt => opt.Ignore())
                .ForMember(d => d.ModifiedAt, opt => opt.Ignore())
                .ForMember(d => d.InvitedBy_TrackerId, opt => opt.Ignore())
                .ForMember(d => d.InvitedByTrackerId, opt => opt.Ignore())
                .ForMember(d => d.CheckInBy_TrackerId, opt => opt.Ignore())
                .ForMember(d => d.CheckInByTrackerId, opt => opt.Ignore())
                .ForMember(d => d.Group, opt => opt.Ignore())
                .ForMember(d => d.TrackerRole, opt => opt.Ignore())
                .ForMember(d => d.City, opt => opt.Ignore())
                .ForMember(d => d.Invite, opt => opt.Ignore())
                .ForMember(d => d.Locations, opt => opt.Ignore())
                .ForMember(d => d.TrackerLabels, opt => opt.Ignore());

            #endregion

            #region Event

            CreateMap<DbEvent, Event>();

            CreateMap<Event, DbEvent>()
                .ForMember(d => d.ProfileImage, opt => opt.Ignore())
                .ForMember(d => d.CoverImage, opt => opt.Ignore())
                .ForMember(d => d.CreatedAt, opt => opt.Ignore())
                .ForMember(d => d.UpdatedAt, opt => opt.Ignore())
                .ForMember(d => d.PublishedAt, opt => opt.Ignore())
                .ForMember(d => d.EventType, opt => opt.Ignore())
                .ForMember(d => d.Organizer, opt => opt.Ignore())
                .ForMember(d => d.EventTasks, opt => opt.Ignore())
                .ForMember(d => d.Participants, opt => opt.Ignore());

            #endregion

            #region Label

            CreateMap<DbLabel, Label>();

            CreateMap<Label, DbLabel>()
                .ForMember(d => d.TrackerLabels, opt => opt.Ignore());

            #endregion

            #region EventType

            CreateMap<DbEventType, EventType>();

            CreateMap<EventType, DbEventType>()
                .ForMember(d => d.Created, opt => opt.Ignore())
                .ForMember(d => d.Events, opt => opt.Ignore());

            #endregion

            #region EventSkill

            CreateMap<DbEventSkill, EventSkill>();

            CreateMap<EventSkill, DbEventSkill>()
                .ForMember(d => d.EventTasks, opt => opt.Ignore());

            #endregion

            #region Invite

            CreateMap<DbInvite, Invite>();

            CreateMap<Invite, DbInvite>()
                .ForMember(d => d.Trackers, opt => opt.Ignore());

            #endregion

            #region Group

            CreateMap<DbGroup, Group>();

            CreateMap<Group, DbGroup>()
                .ForMember(d => d.CreatedAt, opt => opt.Ignore())
                .ForMember(d => d.ModifiedAt, opt => opt.Ignore())
                .ForMember(d => d.Trackers, opt => opt.Ignore())
                .ForMember(d => d.UserGroups, opt => opt.Ignore());

            #endregion

            #region City

            CreateMap<DbCity, City>();

            CreateMap<City, DbCity>()
                .ForMember(d => d.Trackers, opt => opt.Ignore());

            #endregion

            #region Location

            CreateMap<DbLocation, Location>();

            CreateMap<Location, DbLocation>()
                .ForMember(d => d.CreatedAt, opt => opt.Ignore())
                .ForMember(d => d.ModifiedAt, opt => opt.Ignore())
                .ForMember(d => d.Tracker, opt => opt.Ignore());

            #endregion

            #region Setting

            CreateMap<DbSetting, Setting>();

            CreateMap<Setting, DbSetting>();

            #endregion

            #region TrackerRole

            CreateMap<DbTrackerRole, TrackerRole>();

            CreateMap<TrackerRole, DbTrackerRole>()
                .ForMember(d => d.CreatedAt, opt => opt.Ignore())
                .ForMember(d => d.Trackers, opt => opt.Ignore());

            #endregion

            #region EventTask

            CreateMap<DbEventTask, EventTask>();

            CreateMap<EventTask, DbEventTask>()
                .ForMember(d => d.CreatedAt, opt => opt.Ignore())
                .ForMember(d => d.UpdatedAt, opt => opt.Ignore())
                .ForMember(d => d.Event, opt => opt.Ignore())
                .ForMember(d => d.EventSkill, opt => opt.Ignore())
                .ForMember(d => d.Assignments, opt => opt.Ignore());

            #endregion

            #region UserSocialAccount

            CreateMap<DbUserSocialAccount, UserSocialAccount>()
                .ForMember(d => d.SocialPlatformName, opt => opt.MapFrom(s => s.SocialPlatform != null ? s.SocialPlatform.Name : null));

            CreateMap<UserSocialAccount, DbUserSocialAccount>()
                .ForMember(d => d.Created, opt => opt.Ignore())
                .ForMember(d => d.Modified, opt => opt.Ignore())
                .ForMember(d => d.User, opt => opt.Ignore())
                .ForMember(d => d.SocialPlatform, opt => opt.Ignore());

            #endregion

            #region Role

            CreateMap<DbRole, Role>();

            CreateMap<Role, DbRole>()
                .ForMember(d => d.UserRoles, opt => opt.Ignore());

            #endregion

            #region UserRole

            CreateMap<DbUserRole, UserRole>()
                .ForMember(d => d.RoleName, opt => opt.MapFrom(s => s.Role != null ? s.Role.Name : null));

            CreateMap<UserRole, DbUserRole>()
                .ForMember(d => d.User, opt => opt.Ignore())
                .ForMember(d => d.Role, opt => opt.Ignore());

            #endregion

            #region UserGroup

            CreateMap<DbUserGroup, UserGroup>()
                .ForMember(d => d.GroupName, opt => opt.MapFrom(s => s.Group != null ? s.Group.Name : null));

            CreateMap<UserGroup, DbUserGroup>()
                .ForMember(d => d.User, opt => opt.Ignore())
                .ForMember(d => d.Group, opt => opt.Ignore());

            #endregion

            #region UserLabel

            CreateMap<DbUserLabel, UserLabel>()
                .ForMember(d => d.LabelName, opt => opt.MapFrom(s => s.Label != null ? s.Label.Name : null));

            CreateMap<UserLabel, DbUserLabel>()
                .ForMember(d => d.User, opt => opt.Ignore())
                .ForMember(d => d.Label, opt => opt.Ignore());

            #endregion

            #region SocialPlatform

            CreateMap<DbSocialPlatform, SocialPlatform>();

            CreateMap<SocialPlatform, DbSocialPlatform>()
                .ForMember(d => d.UserSocialAccounts, opt => opt.Ignore());

            #endregion

            #region OrganizationRole

            CreateMap<DbOrganizationRole, OrganizationRole>();

            CreateMap<OrganizationRole, DbOrganizationRole>()
                .ForMember(d => d.OrganizationUsers, opt => opt.Ignore());

            #endregion

            #region Organization

            CreateMap<DbOrganization, Organization>();

            CreateMap<Organization, DbOrganization>()
                .ForMember(d => d.OwnedBy, opt => opt.Ignore())
                .ForMember(d => d.CreatedBy, opt => opt.Ignore())
                .ForMember(d => d.ModifiedBy, opt => opt.Ignore())
                .ForMember(d => d.CreatedAt, opt => opt.Ignore())
                .ForMember(d => d.ModifiedAt, opt => opt.Ignore())
                .ForMember(d => d.Owner, opt => opt.Ignore())
                .ForMember(d => d.Creator, opt => opt.Ignore())
                .ForMember(d => d.Modifier, opt => opt.Ignore())
                .ForMember(d => d.OrganizationUsers, opt => opt.Ignore());

            #endregion

            #region OrganizationUser

            CreateMap<DbOrganizationUser, OrganizationUser>();

            CreateMap<OrganizationUser, DbOrganizationUser>()
                .ForMember(d => d.AssingedAt, opt => opt.Ignore())
                .ForMember(d => d.User, opt => opt.Ignore())
                .ForMember(d => d.OrganizationRole, opt => opt.Ignore())
                .ForMember(d => d.AssignedByUser, opt => opt.Ignore());

            #endregion
        }
    }
}
