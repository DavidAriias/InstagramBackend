namespace Instagram.Infraestructure.Services.Scheduling
{
    public interface IQuartzJobScheduler
    {
        public Task ScheduleStoryCleanupJob();

    }
}
