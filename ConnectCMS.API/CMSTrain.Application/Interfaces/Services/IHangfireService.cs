using CMSTrain.Domain.Common.Enum;
using CMSTrain.Application.Common.Service;

namespace CMSTrain.Application.Interfaces.Services;

public interface IHangfireService : ITransientService
{
    void HandleRecurringJob(string recurringJobId, string cron, Scheduler schedulerTrigger);

    void RemoveRecurringJobs(string prefix);
}