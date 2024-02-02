using ElementaryElevatorControlSystem.Enums;
using ElementaryElevatorControlSystem.Models;

namespace ElementaryElevatorControlSystem.Tests.Models
{
    public class BuildingTests
    {
        [Fact]
        public void Building_SetupElevators_ShouldCreateCorrectNumberOfElevators()
        {
            // Arrange
            int totalFloors = 10;
            int numberOfElevators = 4;

            // Act
            var building = new Building(totalFloors, numberOfElevators);

            // Assert
            Assert.Equal(numberOfElevators, building.Elevators.Count);
            for (int i = 0; i < numberOfElevators; i++)
            {
                Assert.Equal(Building.GROUND_FLOOR, building.Elevators[i].CurrentFloor);
                Assert.Equal(Direction.Up, building.Elevators[i].CurrentDirection);
                Assert.Equal(State.Idle, building.Elevators[i].CurrentState);
                Assert.Equal("car " + (i + 1), building.Elevators[i].Label);
            }
        }
    }
}
