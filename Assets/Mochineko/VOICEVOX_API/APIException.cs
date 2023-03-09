#nullable enable
namespace Mochineko.VOICEVOX_API
{
    public sealed class APIException : System.Exception
    {
        internal APIException(APIError error)
            : base(message: $"Location:{error.Detail[0].Location}, " +
                            $"Message:{error.Detail[0].Message}, " +
                            $"ErrorType:{error.Detail[0].ErrorType}.")
        {
        }
    }
}