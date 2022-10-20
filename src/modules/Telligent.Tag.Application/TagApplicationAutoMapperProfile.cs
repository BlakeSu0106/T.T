using AutoMapper;
using Telligent.Tag.Application.Dtos.BatchTransactionLog;
using Telligent.Tag.Application.Dtos.BehaviorTagCategory;
using Telligent.Tag.Application.Dtos.CustomizationTagCategory;
using Telligent.Tag.Application.Dtos.Event;
using Telligent.Tag.Application.Dtos.EventTag;
using Telligent.Tag.Application.Dtos.SystemEvent;
using Telligent.Tag.Application.Dtos.Tag;
using Telligent.Tag.Application.Dtos.TagCategoryPermission;
using Telligent.Tag.Application.Dtos.TagTracking;
using Telligent.Tag.Domain.Tags;

namespace Telligent.Tag.Application;

public class TagApplicationAutoMapperProfile : Profile
{
    public TagApplicationAutoMapperProfile()
    {
        ShouldMapProperty = prop =>
            prop.GetMethod is not null && (prop.GetMethod.IsAssembly || prop.GetMethod.IsPublic);

        CreateMap<BehaviorTagCategory, BehaviorTagCategoryDto>();
        CreateMap<CreateBehaviorTagCategoryDto, BehaviorTagCategory>();
        CreateMap<UpdateBehaviorTagCategoryDto, BehaviorTagCategory>();

        CreateMap<BatchTransactionLog, BatchTransactionLogDto>();
        CreateMap<CreateBatchTransactionLogDto, BatchTransactionLog>();

        CreateMap<CustomizationTagCategory, CustomizationTagCategoryDto>();
        CreateMap<CreateCustomizationTagCategoryDto, CustomizationTagCategory>();
        CreateMap<UpdateCustomizationTagCategoryDto, CustomizationTagCategory>();

        CreateMap<Event, EventDto>();
        CreateMap<CreateEventDto, Event>();
        CreateMap<UpdateEventDto, Event>();

        CreateMap<EventTag, EventTagDto>();
        CreateMap<CreateEventTagDto, EventTag>();

        CreateMap<SystemEvent, SystemEventDto>();
        CreateMap<SystemEventDto, SystemEvent>();

        CreateMap<Domain.Tags.Tag, TagDto>();
        CreateMap<TagDto, Domain.Tags.Tag>();
        CreateMap<CreateTagDto, Domain.Tags.Tag>();
        CreateMap<UpdateTagDto, Domain.Tags.Tag>();

        CreateMap<TagCategoryPermission, TagCategoryPermissionDto>();
        CreateMap<CreateTagCategoryPermissionDto, TagCategoryPermission>();
        CreateMap<UpdateTagCategoryPermissionDto, TagCategoryPermission>();

        CreateMap<TagCategoryPermission, TagCategoryPermissionDto>();
        CreateMap<CreateTagCategoryPermissionDto, TagCategoryPermission>();
        CreateMap<UpdateTagCategoryPermissionDto, TagCategoryPermission>();

        CreateMap<TagTracking, TagTrackingDto>();
        CreateMap<CreateTagTrackingDto, TagTracking>();
    }
}