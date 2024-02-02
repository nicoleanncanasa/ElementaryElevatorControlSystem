using ElementaryElevatorControlSystem.Controllers;
using ElementaryElevatorControlSystem.Enums;
using ElementaryElevatorControlSystem.Services.Interfaces;
using Moq;

namespace ElementaryElevatorControlSystem.Tests.Controller
{
    public class ElevatorControllerTests
    {
        private readonly Mock<IElevatorService> _mockElevatorService;
        private readonly Mock<ILoggerService> _mockLoggerService;
        private readonly ElevatorController _controller;

        public ElevatorControllerTests()
        {
            _mockElevatorService = new Mock<IElevatorService>();
            _mockLoggerService = new Mock<ILoggerService>();
            _controller = new ElevatorController(_mockElevatorService.Object, _mockLoggerService.Object);
        }

        #region RequestElevatorAsync
        [Fact]
        public async Task RequestElevatorAsync_ValidRequest_ShouldLogInfo()
        {
            // Arrange
            int floor = 5;
            Direction direction = Direction.Up;

            // Act
            await _controller.RequestElevatorAsync(floor, direction);

            // Assert
            _mockElevatorService.Verify(s => s.CallElevatorAsync(floor, direction), Times.Once);
            _mockLoggerService.Verify(l => l.LogInfoAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task RequestElevatorAsync_ServiceThrowsException_ShouldLogError()
        {
            // Arrange
            _mockElevatorService.Setup(s => s.CallElevatorAsync(It.IsAny<int>(), It.IsAny<Direction>()))
                                .ThrowsAsync(new InvalidOperationException("Test exception"));

            int floor = 5;
            Direction direction = Direction.Up;

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.RequestElevatorAsync(floor, direction));
            _mockLoggerService.Verify(l => l.LogErrorAsync(It.Is<string>(msg => msg.Contains("Test exception"))), Times.Once);
        }
        #endregion

        #region MoveElevatorsAsync
        [Fact]
        public async Task MoveElevatorsAsync_WhenCalled_ShouldInvokeServiceMethod()
        {
            // Arrange
            // Setup already done in constructor

            // Act
            await _controller.MoveElevatorsAsync();

            // Assert
            _mockElevatorService.Verify(s => s.MoveElevatorsAsync(), Times.Once);
        }

        [Fact]
        public async Task MoveElevatorsAsync_WhenServiceThrowsException_ShouldLogError()
        {
            // Arrange
            _mockElevatorService.Setup(s => s.MoveElevatorsAsync()).ThrowsAsync(new InvalidOperationException("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.MoveElevatorsAsync());
            _mockLoggerService.Verify(l => l.LogErrorAsync(It.Is<string>(msg => msg.Contains("Test exception"))), Times.Once);
        }

        #endregion

        #region DisplayStatusAsync
        [Fact]
        public async Task DisplayStatusAsync_WhenCalled_ShouldRetrieveAndLogStatus()
        {
            // Arrange
            string expectedStatus = "car 1 is on floor 1 - idle";
            _mockElevatorService.Setup(s => s.GetElevatorStatus()).ReturnsAsync(expectedStatus);

            // Act
            await _controller.DisplayStatusAsync();

            // Assert
            _mockElevatorService.Verify(s => s.GetElevatorStatus(), Times.Once);
            _mockLoggerService.Verify(l => l.LogInfoAsync(expectedStatus), Times.Once);
        }

        [Fact]
        public async Task DisplayStatusAsync_WhenServiceThrowsException_ShouldLogError()
        {
            // Arrange
            _mockElevatorService.Setup(s => s.GetElevatorStatus()).ThrowsAsync(new InvalidOperationException("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.DisplayStatusAsync());
            _mockLoggerService.Verify(l => l.LogErrorAsync(It.Is<string>(msg => msg.Contains("Test exception"))), Times.Once);
        }
        #endregion

    }
}
