using System.Configuration;

namespace Scheduler.Configuration
{
    public class SchedulerTimeTableCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new SchedulerTimeTableItem();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SchedulerTimeTableItem)element).TaskName;
        }
    }
}