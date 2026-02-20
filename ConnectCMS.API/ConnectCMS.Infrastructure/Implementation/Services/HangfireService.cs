using Hangfire;
using Hangfire.Storage;
using CMSTrain.Application.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using SchedulerModel = CMSTrain.Domain.Common.Enum.Scheduler;

namespace CMSTrain.Infrastructure.Implementation.Services;

public class HangfireService(IServiceScopeFactory serviceScopeFactory) : IHangfireService
{
    public void HandleRecurringJob(string recurringJobId, string cron, SchedulerModel schedulerTrigger)
    {
        var jobOptions = new RecurringJobOptions
        {
            TimeZone = TimeZoneInfo.Utc,
            MisfireHandling = MisfireHandlingMode.Strict
        };
        
        using var scope = serviceScopeFactory.CreateScope();

        switch (schedulerTrigger)
        {
            case SchedulerModel.PersonalAssessment:
            {
                var trainingInspectionService = scope.ServiceProvider.GetRequiredService<ITrainingInspectionService>();

                var trainingInspectionConfiguration = recurringJobId.Split('%')[0].Trim();
            
                RecurringJob.AddOrUpdate(recurringJobId, 
                    () => trainingInspectionService.TriggerTrainingInspectionQuestionnaireForSubordinates(new Guid(trainingInspectionConfiguration), recurringJobId), 
                    FormatCronExpression(cron), 
                    jobOptions);
                
                break;
            }
            case SchedulerModel.CertificationTrigger:
            {
                var certificationService = scope.ServiceProvider.GetRequiredService<ICertificationService>();

                var training = recurringJobId.Trim();
            
                RecurringJob.AddOrUpdate(recurringJobId, 
                    () => certificationService.IssueTrainingCertifications(new Guid(training)), 
                    FormatCronExpression(cron), 
                    jobOptions);
                
                break;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(schedulerTrigger), schedulerTrigger, null);
        }
    }
    
    public void RemoveRecurringJobs(string prefix)
    {
        using var connection = JobStorage.Current.GetConnection();
        
        var recurringJobs = connection.GetRecurringJobs();

        var jobsToRemove = recurringJobs.Where(job => job.Id.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));

        foreach (var job in jobsToRemove)
        {
            RecurringJob.RemoveIfExists(job.Id);
        }
    }
    
    private static string FormatCronExpression(string originalCron)
    {
        try
        {
            var parts = originalCron.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 6) throw new ArgumentException("Invalid cron expression format");
            
            var year = int.Parse(parts[5]);
            var month = int.Parse(parts[3]);
            var day = int.Parse(parts[2]);
            
            var date = new DateTime(year, month, day);
            
            var dayOfWeek = (int)date.DayOfWeek;
            
            var result = $"{parts[0]} {parts[1]} {parts[2]} {parts[3]} {dayOfWeek}";
            
            return result;
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Failed to format cron expression: {ex.Message}", ex);
        }
    }
}