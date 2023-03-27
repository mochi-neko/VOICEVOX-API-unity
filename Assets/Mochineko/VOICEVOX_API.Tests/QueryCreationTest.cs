#nullable enable
using System.Threading;
using System.Threading.Tasks;
using Mochineko.VOICEVOX_API.QueryCreation;
using FluentAssertions;
using Mochineko.Relent.UncertainResult;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.VOICEVOX_API.Tests
{
    [TestFixture]
    internal sealed class QueryCreationTest
    {
        [Test]
        [RequiresPlayMode(false)]
        public async Task CreateQuery()
        {
            var createQueryResult = await QueryCreationAPI.CreateQueryAsync(
                HttpClientPool.PooledClient,
                text: "テスト",
                speaker: 0,
                coreVersion: null,
                CancellationToken.None);

            createQueryResult.Success.Should().BeTrue();
        }
        
        [Test]
        [RequiresPlayMode(false)]
        public async Task CreateQueryFromPreset()
        {
            var createQueryResult = await QueryCreationAPI.CreateQueryFromPresetAsync(
                HttpClientPool.PooledClient,
                text: "テスト",
                presetId: 0,
                coreVersion: null,
                CancellationToken.None);

            createQueryResult.Success.Should().BeTrue();
        }
    }
}