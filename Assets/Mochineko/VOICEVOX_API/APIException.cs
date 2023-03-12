#nullable enable
using System;
using System.Net;

namespace Mochineko.VOICEVOX_API
{
    /// <summary>
    /// An exception on VOICEVOX API.
    /// </summary>
    public sealed class APIException : System.Exception
    {
        internal APIException(string response, HttpStatusCode statusCode, Exception innerException)
            : base(
                message: BuildMessage(response, statusCode),
                innerException:innerException)
        {
        }

        private static string BuildMessage(string response, HttpStatusCode statusCode)
            => $"[VOICEVOX_API] ({(int)statusCode}:{statusCode}) {response}.";
    }
}