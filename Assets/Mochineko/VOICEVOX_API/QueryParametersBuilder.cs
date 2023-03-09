#nullable enable
using System.Collections.Generic;
using System.Text;

namespace Mochineko.VOICEVOX_API
{
    internal static class QueryParametersBuilder
    {
        public static string Build(IReadOnlyList<(string key, string value)> parameters)
        {
            var builder = new StringBuilder();

            builder.Append('?');

            for (var i = 0; i < parameters.Count; i++)
            {
                var parameter = parameters[i];
                builder.Append($"{parameter.key}={parameter.value}");

                if (i != parameters.Count - 1)
                {
                    builder.Append('&');
                }
            }

            return builder.ToString();
        }
    }
}