using AdminAPI.DTOs.About;
using AdminAPI.DTOs.MasgedSettings;
using AdminAPI.DTOs.Activities;
using AdminAPI.DTOs.Activity;
using AdminAPI.DTOs.ContactInfo;
using AdminAPI.DTOs.TipGuidance;
using AdminAPI.DTOs.Expensives;
using AdminAPI.DTOs.HeroSlides;
using AdminAPI.DTOs.Mosques;
using AdminAPI.DTOs.PlanLevels;
using AdminAPI.Helpers;
using AdminAPI.DTOs.News;
using AdminAPI.DTOs.FilesManager;
using AdminAPI.DTOs.SocialLink;
using AdminAPI.DTOs.TeacherSalaries;
using AdminAPI.DTOs.TeacherSendNotes;
using AdminAPI.Models;
using AutoMapper;

namespace AdminAPI.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<AboutAssociation, AboutDto>();
        CreateMap<MasgedSetting, MasgedSettingsDto>();
        CreateMap<UpdateAboutRequestDto, AboutAssociation>()
            .ForMember(d => d.Id, o => o.Ignore());
        CreateMap<Activity, ActivityListItemDto>();
        CreateMap<Activity, ActivityDto>();
        CreateMap<TipGuidance, TipGuidanceDto>();
        CreateMap<TipGuidance, TipGuidanceListItemDto>();
        CreateMap<ContactInfo, ContactInfoDto>();
        CreateMap<ContactInfo, ContactInfoListItemDto>();
        CreateMap<HeroSlide, HeroSlideListItemDto>();
        CreateMap<HeroSlide, HeroSlideDto>();
        CreateMap<NewsItem, NewsDto>();
        CreateMap<NewsItem, NewsListItemDto>();
        CreateMap<Mosque, MosqueDto>();
        CreateMap<Mosque, MosqueListItemDto>();
        CreateMap<PlanLevel, PlanLevelDto>()
            .ForMember(d => d.UnitTypeDisplay, o => o.MapFrom(s => PlanLevelUnitDisplay.GetUnitDisplay(s.UnitType)))
            .ForMember(d => d.IsGlobal, o => o.MapFrom(s => s.CreatedByTeacherId == null));
        CreateMap<PlanLevel, PlanLevelListItemDto>()
            .ForMember(d => d.UnitTypeDisplay, o => o.MapFrom(s => PlanLevelUnitDisplay.GetUnitDisplay(s.UnitType)))
            .ForMember(d => d.IsGlobal, o => o.MapFrom(s => s.CreatedByTeacherId == null));
        CreateMap<SocialLink, SocialLinkDto>();
        CreateMap<SocialLink, SocialLinkListItemDto>();
        CreateMap<Expensive, ExpensiveListItemDto>()
            .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.Teacher != null ? s.Teacher.Name : string.Empty));
        CreateMap<Expensive, ExpensiveDto>();
        CreateMap<FilesManager, FilesManagerDto>();
        CreateMap<FilesManager, FilesManagerListItemDto>();
        CreateMap<TeachersAdminNote, TeacherSendNoteListItemDto>()
            .ForMember(d => d.TeacherName, o => o.MapFrom(s => s.Teacher != null ? s.Teacher.Name : string.Empty));
        CreateMap<TeachersAdminNote, TeacherSendNoteDto>()
            .ForMember(d => d.TeacherName, o => o.MapFrom(s => s.Teacher != null ? s.Teacher.Name : string.Empty));
        CreateMap<Teacher, TeacherOptionDto>();
        CreateMap<TeacherSalary, TeacherSalaryListItemDto>()
            .ForMember(d => d.TeacherName, o => o.MapFrom(s => s.Teacher != null ? s.Teacher.Name : string.Empty))
            .ForMember(d => d.Notes, o => o.MapFrom(s => s.Notes ?? string.Empty));
        CreateMap<TeacherSalary, TeacherSalaryDto>()
            .ForMember(d => d.TeacherName, o => o.MapFrom(s => s.Teacher != null ? s.Teacher.Name : string.Empty));
    }
}
