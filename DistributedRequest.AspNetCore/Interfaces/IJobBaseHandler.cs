using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using DistributedRequest.AspNetCore.Models;

namespace DistributedRequest.AspNetCore.Interfaces
{
    public interface IJobBaseHandler
    {
        Task<ReturnT> Execute(JobContext context, CancellationToken cancellationToken);
    }
}
