using Ninject.Modules;
using Quartz;
using Scheduler.Jobs;

namespace Scheduler.Infrastructure
{
    public class JobsModule : NinjectModule
    {
        public override void Load()
        {
            Kernel?.Bind<IJob>().To<NotifyJob>().Named("TestTask");
        }
    }
}