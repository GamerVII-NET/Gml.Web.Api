using AutoMapper;
using FluentValidation;
using Gml.Web.Api.Core.Services;
using Gml.Web.Api.Dto.Profile;
using GmlCore.Interfaces;

namespace Gml.Web.Api.Core.Handlers;

public interface IProfileHandler
{
    static abstract Task<IResult> GetProfiles(IMapper mapper, IGmlManager gmlManager);

    static abstract Task<IResult> CreateProfile(
        HttpContext context,
        ISystemService systemService,
        IMapper mapper,
        IGmlManager gmlManager,
        IValidator<ProfileCreateDto> validator);

    static abstract Task<IResult> UpdateProfile(
        HttpContext context,
        ISystemService systemService,
        IMapper mapper,
        IGmlManager gmlManager,
        IValidator<ProfileUpdateDto> validator);

    static abstract Task<IResult> RestoreProfile(
        IMapper mapper,
        IGmlManager gmlManager,
        IValidator<ProfileRestoreDto> validator,
        ProfileRestoreDto profileName);

    static abstract Task<IResult> CompileProfile(
        IMapper mapper,
        IGmlManager gmlManager,
        IValidator<CompileProfileDto> validator,
        CompileProfileDto profileName);

    static abstract Task<IResult> GetProfileInfo(
        IMapper mapper,
        IGmlManager gmlManager,
        IValidator<ProfileCreateInfoDto> validator,
        ProfileCreateInfoDto createInfoDto);

    static abstract Task<IResult> RemoveProfile(
        IGmlManager gmlManager,
        string profileName,
        bool removeFiles);
}
