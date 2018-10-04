using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scheduler.Infrastructure.Extensions
{
    public static class ExceptionExtensions
    {
        public static string ToCompleteMessage(this Exception exception)
        {
            var b = new StringBuilder();
            GetExceptions(exception).ToList()
                .ForEach(x => b.AppendFormat("{0}{1}", x.Message, Environment.NewLine));
            return b.ToString();
        }

        private static IEnumerable<Exception> GetExceptions(Exception exception)
        {
            while (true)
            {
                yield return exception;
                if (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                    continue;
                }
                break;
            }
        }
    }
}