namespace Scheduler.Infrastructure.Abstract
{
    public interface IModuleController
    {
        void TestForWorkingPossibility();
        void SetWorkStatusToImpossible();
    }
}