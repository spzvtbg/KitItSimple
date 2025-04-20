namespace KitItSimple.DbClient
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

        internal static void Log(this object content)
        {
            if (loggingAction != null)
            {
                loggingAction($"{content}");
            }
        }

        internal static void Log(this IDbCommand command)
        {
            if (loggingAction != null)
            {
                loggingAction($"Executed: {command.CommandText}");

                if (command.Parameters.Count > 0)
                {
                    var content = "With parameters:";

                    foreach (IDataParameter parameter in command.Parameters)
                    {
                        content += Environment.NewLine;
                        content += $"  - @{parameter.ParameterName} = {parameter.Value} of type: {parameter.DbType}";
                    }

                    loggingAction(content);
                }

                loggingAction(new string('=', 30));
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
