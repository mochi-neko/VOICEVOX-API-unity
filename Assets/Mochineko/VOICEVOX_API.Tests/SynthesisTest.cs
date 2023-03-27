#nullable enable
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Mochineko.VOICEVOX_API.QueryCreation;
using Mochineko.VOICEVOX_API.Synthesis;
using FluentAssertions;
using Mochineko.Relent.UncertainResult;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.VOICEVOX_API.Tests
{
    [TestFixture]
    internal sealed class SynthesisTest
    {
        [Test]
        [RequiresPlayMode(false)]
        public async Task Synthesis()
        {
            var createQueryResult = await QueryCreationAPI.CreateQueryAsync(
                HttpClientPool.PooledClient,
                text: "テスト",
                speaker: 0,
                coreVersion: null,
                CancellationToken.None);

            if (createQueryResult is IUncertainSuccessResult<AudioQuery> success)
            {
                var synthesizeResult = await SynthesisAPI.SynthesizeAsync(
                    HttpClientPool.PooledClient,
                    audioQuery: success.Result,
                    speaker: 0,
                    enableInterrogativeUpspeak: null,
                    coreVersion: null,
                    CancellationToken.None);

                synthesizeResult.Success.Should().BeTrue();
                await using var _ = (synthesizeResult as IUncertainSuccessResult<Stream>)?.Result;
            }
            else
            {
                Assert.Fail();
            }
        }
        
        [Test]
        [RequiresPlayMode(false)]
        [Ignore("Experimental feature")]
        public async Task CancellableSynthesis()
        {
            var createQueryResult = await QueryCreationAPI.CreateQueryAsync(
                HttpClientPool.PooledClient,
                text: "テスト",
                speaker: 0,
                coreVersion: null,
                CancellationToken.None);

            if (createQueryResult is IUncertainSuccessResult<AudioQuery> success)
            {
                var synthesizeResult = await SynthesisAPI.CancellableSynthesizeAsync(
                    HttpClientPool.PooledClient,
                    audioQuery: success.Result,
                    speaker: 0,
                    coreVersion: null,
                    CancellationToken.None);

                synthesizeResult.Success.Should().BeTrue();
                await using var _ = (synthesizeResult as IUncertainSuccessResult<Stream>)?.Result;
            }
            else
            {
                Assert.Fail();
            }
        }
    }
}