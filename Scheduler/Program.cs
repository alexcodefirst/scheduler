using System;
using Scheduler.Infrastructure.Abstract;
using Scheduler.Infrastructure.Concrete;
using Scheduler.Quartz;
using Ninject;
using NLog;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Scheduler.Infrastructure;
using Topshelf;

namespace Scheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            var kernel = CreateNinjectKernel();
            HostFactory.Run(x =>
            {
                x.Service<ServiceWorker>(s =>
                {
                    s.ConstructUsing(name => new ServiceWorker(kernel));
                    s.WhenStarted((tc, hc) => tc.Start(hc));
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("Job scheduling system");
                x.SetDisplayName("YourProjectsNameScheduler");
                x.SetServiceName("YourProjectsNameScheduler");

                x.StartAutomatically();

                x.EnableServiceRecovery(s =>
                {
                    s.RestartService(0);
                    s.SetResetPeriod(0);
                    s.OnCrashOnly();
                });
            });
            Console.ReadKey();
        }

        private static IKernel CreateNinjectKernel()
        {
            var kernel = new StandardKernel(new JobsModule());
            #region NLog
            kernel.Bind<ILogger>().ToMethod(
              context =>
              {
                  if (context.Request.Target != null)
                      if (context.Request.Target.Member.DeclaringType != null)
                          return LogManager.GetLogger(context.Request.Target.Member.DeclaringType.ToString());
                  return LogManager.GetLogger("UnknownTarget");
              });
            #endregion

            kernel.Bind<IScheduler>().ToMethod(context => StdSchedulerFactory.GetDefaultScheduler());
            kernel.Bind<IJobFactory>().To<NinjectJobFactory>();
            kernel.Bind<IModuleController>().To<ModuleController>().InSingletonScope();

            return kernel;
        }
    }
}
