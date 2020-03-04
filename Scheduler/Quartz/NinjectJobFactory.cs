using System;
using System.Linq;
using Ninject;
using Quartz;
using Quartz.Spi;
using NLog;

namespace Scheduler.Quartz
{
    public class NinjectJobFactory : IJobFactory
    {
        private readonly object _locker = new object();
        private readonly ILogger _logger;
        private readonly IKernel _kernel;

        public NinjectJobFactory(IKernel kernel, ILogger logger)
        {
            this._kernel = kernel;
            this._logger = logger;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            lock (_locker)
            {
                var jobName = bundle.JobDetail.Key.Name;

                try
                {
                    var isJobExcecuting = scheduler.GetCurrentlyExecutingJobs().Any(x => x.JobDetail.Key.Name == jobName);

                    if (isJobExcecuting)
                    {
                        throw new Exception("The job already working");
                    }

                    return _kernel.Get<IJob>(jobName);
                }
                catch (Exception ex)
                {
                    _logger.Warn("The job '{0}' could not be extracted, considered to skip ({1})", jobName, ex.Message);

                    return null;
                }
            }
        }

        public void ReturnJob(IJob job)
        {
            //throw new System.NotImplementedException();
        }
    }
}