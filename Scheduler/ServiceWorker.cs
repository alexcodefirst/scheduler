using System;
using System.Configuration;
using Scheduler.Configuration;
using Scheduler.Infrastructure.Concrete;
using Ninject;
using Quartz;
using Quartz.Spi;
using Topshelf;
using NLog;

namespace Scheduler
{
    public class ServiceWorker
    {
        private readonly ILogger _logger;
        private readonly IScheduler _scheduler;
        private readonly ConfigFileWatcher _watcher;

        public ServiceWorker(IKernel kernel)
        {
            _logger = kernel.Get<ILogger>();
            _scheduler = kernel.Get<IScheduler>();
            _scheduler.JobFactory = kernel.Get<IJobFactory>();
            _watcher = new ConfigFileWatcher(_logger);

            _logger.Info("Scheduler service worker was initialized");

            AppDomain.CurrentDomain.FirstChanceException += (sender, eventArgs) =>
            {
                _logger.Error("FirstChanceException:" + eventArgs.Exception.ToString());
            };
        }

        public bool Start(HostControl hc)
        {
            //AddHostControlJob(hc);

            _logger.Info("Reading configuration");

            var connectionManagerDataSection = ConfigurationManager.GetSection(SchedulerTimeTableSection.SectionName) as SchedulerTimeTableSection;
            
            if (connectionManagerDataSection != null)
            {
                _logger.Info("{0} jobs have been read", connectionManagerDataSection.TimeTable.Count);

                _scheduler.Start();

                foreach (var item in connectionManagerDataSection.TimeTable)
                {
                    AddJob((SchedulerTimeTableItem) item);
                }

                _watcher.Start();

                _logger.Info("Scheduler service worker was started");

                return true;
            }
            _logger.Error("Failed to read the job configuration, scheduler will be stopped");

            hc.Stop();

            return true;
        }

        private void AddJob(SchedulerTimeTableItem item)
        {
            _logger.Info("Trying to add '{0}' a job into schedule", item.TaskName);
            try
            {
                var jobDetail = JobBuilder.Create()
                    .WithIdentity(item.TaskName)
                    .Build();
                var jobTrigger = TriggerBuilder.Create()
                    .WithCronSchedule(item.Trigger)
                    .Build();
                _scheduler.ScheduleJob(jobDetail, jobTrigger );
                _logger.Info("The Job was '{0}' added successfully", item.TaskName);
            }
            catch (Exception ex)
            {
                _logger.Error("Job Error: {0}", ex.Message);
            }
        }

        public void Stop()
        {
            _watcher.Stop();
            _scheduler.Shutdown();
            _logger.Info("Scheduler service worker was stopped");
        }
    }
}