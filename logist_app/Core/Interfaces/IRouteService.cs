using logist_app.Core.Entities;
using logist_app.Infrastructure.Service.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace logist_app.Core.Interfaces
{
    public interface IRouteService
    {
        Task<RouteBuildResult> BuildRoute(List<int> clientsId);

    }
}
