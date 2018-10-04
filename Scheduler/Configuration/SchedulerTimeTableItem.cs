using System.Configuration;

namespace Scheduler.Configuration
{
    public class SchedulerTimeTableItem : ConfigurationElement
    {
        [ConfigurationProperty("trigger", IsRequired = true)]
        public string Trigger
        {
            get { return (string)this["trigger"]; }
            set { this["trigger"] = value; }
        }

        [ConfigurationProperty("taskname", IsRequired = true)]
        public string TaskName
        {
            get { return (string)this["taskname"]; }
            set { this["taskname"] = value; }
        }
    }
}