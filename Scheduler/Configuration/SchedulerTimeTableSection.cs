using System.Configuration;

namespace Scheduler.Configuration
{
    public class SchedulerTimeTableSection : ConfigurationSection
    {
        public const string SectionName = "SchedulerTimeTableSection";
        private const string TimeTableName = "TimeTable";

        [ConfigurationProperty(TimeTableName)]
        [ConfigurationCollection(typeof(SchedulerTimeTableCollection), AddItemName = "add")]
        public SchedulerTimeTableCollection TimeTable => (SchedulerTimeTableCollection)base[TimeTableName];
    }
}