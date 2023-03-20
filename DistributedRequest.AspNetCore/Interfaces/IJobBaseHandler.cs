using DistributedRequest.AspNetCore.Models;
using System.Threading;
using System.Threading.Tasks;

namespace DistributedRequest.AspNetCore.Interfaces
{
    public interface IJobBaseHandler
    {
        Task<ReturnT> Execute(JobContext context, CancellationToken cancellationToken);
    }
}
