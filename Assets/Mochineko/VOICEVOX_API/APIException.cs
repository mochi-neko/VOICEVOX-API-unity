#nullable enable
namespace Mochineko.VOICEVOX_API
{
    /// <summary>
    /// An exception on VOICEVOX API.
    /// </summary>
    public sealed class APIException : System.Exception
    {
        internal APIException(APIErrorResponseBody errorResponseBody)
            : base(message: $"Location:{errorResponseBody.Detail[0].Location}, " +
                            $"Message:{errorResponseBody.Detail[0].Message}, " +
                            $"ErrorType:{errorResponseBody.Detail[0].ErrorType}.")
        {
        }
    }
}