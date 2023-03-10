#nullable enable
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Mochineko.VOICEVOX_API.QueryCreation;
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
            var query = await QueryCreationAPI.CreateQueryAsync(
                text: "テスト",
                speaker: 0,
                coreVersion: null,
                CancellationToken.None);

            query.Should().NotBeNull();
        }
        
        [Test]
        [RequiresPlayMode(false)]
        public async Task CreateQueryFromPreset()
        {
            var query = await QueryCreationAPI.CreateQueryFromPresetAsync(
                text: "テスト",
                presetId: 0,
                coreVersion: null,
                CancellationToken.None);

            query.Should().NotBeNull();
        }
    }
}