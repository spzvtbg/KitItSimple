namespace GenericDbClient
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;

    public static class DBClient
    {
        private static readonly Dictionary<string, MethodInfo> tryParseMethods
              = new Dictionary<string, MethodInfo>();
        private static Action<string> loggingAction;

        public static void AddLogger(Action<string> loggingHandler)
            => loggingAction = loggingHandler;

        public static void AddOrUpdateTryParseMethod<T>(MethodInfo tryParseMethod)
            => tryParseMethods[typeof(T).Name] = tryParseMethod;

        public static T TryParseValueOrDefault<T>(this object value)
        {
            if (value is null)
            {
                return default(T);
            }

            if (value is T targetValue)
            {
                return targetValue;
            }

            var targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
            var tryParseMethod = TryGetTryParseMethod(targetType);
            var tryParseParameters = new object[2];
            tryParseParameters[0] = $"{value}";
            tryParseParameters[1] = default(T);
            _ = tryParseMethod?.Invoke(null, tryParseParameters);

            return tryParseParameters[1] is T parsedValue ? parsedValue : default;
        }

        internal static void Log(this object content) => LogContent(content);

        internal static void Log(this IDbCommand sqlCommand)
        {
            if (loggingAction != null)
            {
                var content = $"Executed: {sqlCommand.CommandText}";

                if (sqlCommand.Parameters.Count > 0)
                {
                    content += " Parameters:";

                    foreach (IDataParameter parameter in sqlCommand.Parameters)
                    {
                        content += $" @{parameter.ParameterName} = {parameter.Value} of type: {parameter.DbType};";
                    }

                    LogContent(content);
                }
            }
        }

        private static void LogContent(object content)
        {
            if (loggingAction != null)
            {
                loggingAction($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fffffff}|{content}");
            }
        }

        private static MethodInfo TryGetTryParseMethod(Type targetType)
        {
            if (!tryParseMethods.TryGetValue(targetType.Name, out var tryParseMethod))
            {
                var tryParseMethodName = nameof(int.TryParse);

                tryParseMethod = targetType
                    .GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .FirstOrDefault(x => x.Name == tryParseMethodName && x.IsGenericMethod);

                if (tryParseMethod is null && targetType.IsEnum)
                {
                    tryParseMethod = typeof(Enum)
                        .GetMethods(BindingFlags.Public | BindingFlags.Static)
                        .FirstOrDefault(x => x.Name == tryParseMethodName && x.IsGenericMethod)
                        ?.MakeGenericMethod(targetType);
                }

                tryParseMethods[targetType.Name] = tryParseMethod;
            }

            return tryParseMethod;
        }
    }
}
