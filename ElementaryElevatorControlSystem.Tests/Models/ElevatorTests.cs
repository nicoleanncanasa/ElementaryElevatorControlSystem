using ElementaryElevatorControlSystem.Enums;
using ElementaryElevatorControlSystem.Models;

namespace ElementaryElevatorControlSystem.Tests.Models
{
    public class ElevatorTests
    {
        [Fact]
        public void AddStop_ShouldAddUniqueStopsOnly()
        {
            // Arrange
            var elevator = new Elevator();
            int stopFloor1 = 3;
            int stopFloor2 = 5;

            // Act
            elevator.AddStop(stopFloor1);
            elevator.AddStop(stopFloor1);
            elevator.AddStop(stopFloor2);

            // Assert
            var stops = elevator.ReadOnlyStops;
            Assert.Equal(2, stops.Count); 
            Assert.Contains(stopFloor1, stops);
            Assert.Contains(stopFloor2, stops);
        }

        [Fact]
        public async Task MoveToNextFloorAsync_ShouldUpdateFloorAndState()
        {
            // Arrange
            int highestFloor = 10;
            var elevator = new Elevator();
            elevator.AddStop(2); 

            // Act
            await elevator.MoveToNextFloorAsync(highestFloor); 

            // Assert
            Assert.Equal(2, elevator.CurrentFloor);
            Assert.Equal(Direction.Up, elevator.CurrentDirection); 
            Assert.Equal(State.Idle, elevator.CurrentState);
            Assert.DoesNotContain(2, elevator.ReadOnlyStops); 
        }

        [Fact]
        public async Task MoveToNextFloorAsync_WithNoStops_ShouldRemainIdle()
        {
            // Arrange
            int highestFloor = 10;
            var elevator = new Elevator();

            // Act
            await elevator.MoveToNextFloorAsync(highestFloor);

            // Assert
            Assert.Equal(1, elevator.CurrentFloor); 
            Assert.Equal(State.Idle, elevator.CurrentState); 
        }
    }
}
