#nullable enable
using System.Threading;
using System.Threading.Tasks;
using Mochineko.VOICEVOX_API.QueryCreation;
using Mochineko.VOICEVOX_API.Synthesis;
using FluentAssertions;
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
            var query = await QueryCreationAPI.CreateQueryAsync(
                text: "テスト",
                speaker: 0,
                coreVersion: null,
                cancellationToken: CancellationToken.None);

            await using var stream = await SynthesisAPI.SynthesizeAsync(
                query: query,
                speaker: 0,
                enableInterrogativeUpspeak: null,
                coreVersion: null,
                CancellationToken.None);

            stream.Should().NotBeNull();
            stream.CanRead.Should().BeTrue();
            stream.Length.Should().NotBe(0);
        }
        
        [Test]
        [RequiresPlayMode(false)]
        [Ignore("Experimental feature")]
        public async Task CancellableSynthesis()
        {
            var query = await QueryCreationAPI.CreateQueryAsync(
                text: "テスト",
                speaker: 0,
                coreVersion: null,
                cancellationToken: CancellationToken.None);

            await using var stream = await SynthesisAPI.CancellableSynthesizeAsync(
                query: query,
                speaker: 0,
                coreVersion: null,
                CancellationToken.None);

            stream.Should().NotBeNull();
            stream.CanRead.Should().BeTrue();
            stream.Length.Should().NotBe(0);
        }
    }
}