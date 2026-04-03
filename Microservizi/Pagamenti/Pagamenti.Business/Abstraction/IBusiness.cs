using Pagamenti.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pagamenti.Business.Abstraction
{
    public interface IBusiness
    {
        Task CreateVersamentoAsync(VersamentoInsertDto versamentoInsertDto, CancellationToken cancellationToken = default);
    }
}
