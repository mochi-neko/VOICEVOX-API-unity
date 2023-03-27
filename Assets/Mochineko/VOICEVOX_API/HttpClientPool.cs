#nullable enable
using System.Net.Http;

namespace Mochineko.VOICEVOX_API
{
    /// <summary>
    /// Pools <see cref="HttpClient"/> to save socket.
    /// </summary>
    public static class HttpClientPool
    {
        /// <summary>
        /// Pooled <see cref="HttpClient"/>.
        /// </summary>
        public static HttpClient PooledClient { get; } = new();
    }
}