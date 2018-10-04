using System;
using Scheduler.Infrastructure.Abstract;
using Scheduler.Infrastructure.Extensions;
using Quartz;
using NLog;

namespace Scheduler.Jobs
{
    [DisallowConcurrentExecution]
    public class NotifyJob : IJob
    {
        private readonly ILogger _logger;
        private readonly IModuleController _controller;

        public NotifyJob(ILogger logger, IModuleController controller)
        {
            this._logger = logger;
            this._controller = controller;
        }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                _controller.TestForWorkingPossibility();

                _logger.Trace("NotifyJob is working");

                throw new Exception("Test exception");
            }
            catch (Exception ex)
            {
                _logger.Error("NotifyJob failed: {0}", ex.ToCompleteMessage());
            }
        }        
    }
}