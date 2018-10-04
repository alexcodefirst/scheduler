using System;
using System.Threading;
using Scheduler.Infrastructure.Abstract;

namespace Scheduler.Infrastructure.Concrete
{
    public class ModuleController : IModuleController
    {
        private long _flag;

        public void TestForWorkingPossibility()
        {
            if (Interlocked.Read(ref _flag) > 0)
                throw new Exception("The module is about to be stopped, the task cannot be ran");
        }

        public void SetWorkStatusToImpossible()
        {
            Interlocked.Exchange(ref _flag, 1);
        }
    }
}