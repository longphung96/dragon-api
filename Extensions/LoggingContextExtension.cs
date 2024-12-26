using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace DragonAPI.Extensions
{
    public enum LoggingContextEnum
    {
        DragonServiceContext,
        UserContext,
        BattleContext,
        RssContext,
    }
    public static class LoggingContextExtension
    {
        private const string LogContextPrefix = "--log-context";
        private const string RawJsonPrefix = "--raw-json";
        private const string LogEvent = "log_event";
        private const string LogContext = "log_context";
        private const string LogData = "data";
        public static string ToLogContext(this LoggingContextEnum o)
        {
            return $"{LogContextPrefix} {o.ToString()}";
        }
        public static string ToRawJson(this object o)
        {
            return JsonSerializer.Serialize(o);
        }
        private static string ToRawJsonLog(this object o, string logContext = null, string logEvent = null)
        {
            var logObj = new Dictionary<string, object> { };
            if (!string.IsNullOrEmpty(logEvent))
            {
                logObj.TryAdd(logEvent.ToLower(), new Dictionary<string, object> { { LogData, o } });
                logObj.TryAdd(LogEvent, logEvent);
            }
            else
            {
                logObj.TryAdd(LogData, o);
            }
            if (!string.IsNullOrEmpty(logContext))
            {
                logObj.TryAdd(LogContext, logContext.ToLower());
            }
            return $"{RawJsonPrefix} {logObj.ToRawJson()}";
        }

        public static string ToAnalysisLog(this object o, LoggingContextEnum context, string subContext = null, string logEvent = null)
        {
            return $"{context.ToLogContext()} {o.ToRawJsonLog(subContext, logEvent)}";
        }

        public static void LogAnalysis(this ILogger logger, object logData, LoggingContextEnum context, string subContext = null, string logEvent = null)
        {
            logger.LogInformation("{message}", ToAnalysisLog(logData, context, subContext, logEvent));
        }

        public static void LogAnalysis(this ILogger logger, string message)
        {
            logger.LogInformation("{message}", message);
        }
    }
}