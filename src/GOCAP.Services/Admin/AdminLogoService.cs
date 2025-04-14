using GOCAP.Domain;
using GOCAP.Repository.Intention;
using System;
namespace GOCAP.Services;

[RegisterService(typeof(ILogoAdminService))]
public class AdminLogoService(IAdminLogoRepository _logoRepository, IMapper _mapper) : ILogoAdminService
{
    public async Task<LogoUpdate> UpdateAsync(LogoUpdate domain)
    {

        var entity = _mapper.Map<AdminLogoEntity>(domain);

        var resultEntity = await _logoRepository.UpdateAsync(entity);

        return _mapper.Map<LogoUpdate>(resultEntity);
    }

    public async Task<LogoUpdate> GetAsync()
    {
        var entity = await _logoRepository.GetAsync();
        return _mapper.Map<LogoUpdate>(entity);
    }
}
