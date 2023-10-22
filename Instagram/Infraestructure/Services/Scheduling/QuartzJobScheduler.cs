using Quartz.Impl;
using Quartz;
using Instagram.Domain.Services;

namespace Instagram.Infraestructure.Services.Scheduling
{
    public class QuartzJobScheduler :  IQuartzJobScheduler
    {
        public async Task ScheduleStoryCleanupJob()
        {
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            IScheduler scheduler = await schedulerFactory.GetScheduler();
            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<StoryCleanupJob>().Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("StoryCleanupTrigger")
                .WithSimpleSchedule(x => x
                    .WithIntervalInHours(1) // Ejecutar cada hora
                    .RepeatForever())
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }

    }
}
