// NUnit tests for CardScanHandler: 'A' prefix authorizes; always publishes CardScanned; publishes LiftAccessGranted only when authorized.
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using IntegratedSecurity.Application.Abstractions;
using IntegratedSecurity.Application.UseCases;

[TestFixture]
public class CardScanHandlerTests
{
    [Test]
    public async Task A_Prefix_Authorized_Publishes_Both()
    {
        var bus = new Mock<IEventBus>();
        var sut = new CardScanHandler(bus.Object);

        var ok = await sut.HandleAsync("A123", "Reader1", CancellationToken.None);

        Assert.IsTrue(ok);
        bus.Verify(b => b.PublishAsync(It.IsAny<CardScanned>(), It.IsAny<CancellationToken>()), Times.Once);
        bus.Verify(b => b.PublishAsync(It.IsAny<LiftAccessGranted>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
