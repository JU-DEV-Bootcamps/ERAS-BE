using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.DTOs;
using Eras.Domain.Entities;

namespace Eras.Application.Mappers;
public static class ServiceProvidersMapper
{
    public static ServiceProviders ToDomain(this ServiceProvidersDTO Dto)
    {
        ArgumentNullException.ThrowIfNull(Dto);
        return new ServiceProviders
        {
            ServiceProviderName = Dto.ServiceProviderName,
            ServiceProviderLogo = Dto.ServiceProviderLogo,
            Audit = Dto.Audit
        };
    }
    public static ServiceProvidersDTO ToDto(this ServiceProviders Domain)
    {
        ArgumentNullException.ThrowIfNull(Domain);
        return new ServiceProvidersDTO
        {
            ServiceProviderName = Domain.ServiceProviderName,
            ServiceProviderLogo = Domain.ServiceProviderLogo,
            Audit = Domain.Audit
        };
    }
}
