using ElementaryElevatorControlSystem.Enums;
using ElementaryElevatorControlSystem.Models;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ElementaryElevatorControlSystem.Tests.Services
{
    public class ElevatorServiceTests
    {
        private const string ELEVATOR_IDENTIFIER = "car";
        private Building building;
        private ElevatorService elevatorService;

        private void InitializeTestEnvironment(int totalFloors, int numberOfElevators)
        {
            building = new Building(totalFloors, numberOfElevators);
            elevatorService = new ElevatorService(building);
        }

        #region CallElevatorAsync

        [Fact]
        public async Task CallElevatorAsync_ValidFloorAndDirection_ShouldAddStop()
        {
            // Arrange         
            InitializeTestEnvironment(10, 4);
            int requestedFloor = 6;
            Direction direction = Direction.Up;

            // Act
            await elevatorService.CallElevatorAsync(requestedFloor, direction);

            // Assert
            bool stopAdded = building.Elevators.Any(x => x.ReadOnlyStops.Contains(requestedFloor));
            Assert.True(stopAdded, "Stop was not added to any elevator.");
        }

        [Fact]
        public async Task CallElevatorAsync_RequestAboveHighestElevator_ShouldSelectHighestElevator()
        {
            // Arrange
            InitializeTestEnvironment(10, 4);
            int highestFloor = 8;
            var elevator = building.Elevators[3];
            building.Elevators[3].Update(highestFloor, elevator.CurrentDirection, elevator.CurrentState, elevator.Label);

            int requestedFloor = 9;
            Direction direction = Direction.Up;

            // Act
            await elevatorService.CallElevatorAsync(requestedFloor, direction);

            // Assert
            var highestElevator = building.Elevators[3];
            Assert.Contains(requestedFloor, highestElevator.ReadOnlyStops);
        }

        [Fact]
        public async Task CallElevatorAsync_RequestBelowLowestElevator_ShouldSelectLowestElevator()
        {
            // Arrange
            InitializeTestEnvironment(10, 4);
            int lowestFloor = 2;
            Direction currentDirection = Direction.Down;

            var testElevator = building.Elevators[0];
            building.Elevators[0].Update(lowestFloor, currentDirection, testElevator.CurrentState, testElevator.Label);

            int requestedFloor = 1;
            Direction direction = Direction.Down;

            // Act
            await elevatorService.CallElevatorAsync(requestedFloor, direction);

            // Assert
            var lowestElevator = building.Elevators[0];
            Assert.Contains(requestedFloor, lowestElevator.ReadOnlyStops);
        }

        [Fact]
        public async Task CallElevatorAsync_InvalidFloorRequest_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange         
            InitializeTestEnvironment(10, 4);
            int requestedFloor = 15;
            Direction direction = Direction.Up;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => elevatorService.CallElevatorAsync(requestedFloor, direction));
        }
        #endregion

        #region MoveElevatorAsync 

        [Fact]
        public async Task MoveElevatorAsync_AllElevatorsShouldMoveToNextFloor_Success()
        {
            // Arrange         
            InitializeTestEnvironment(10, 4);
            for (int i = 0; i < building.Elevators.Count(); i++)
            {
                building.Elevators[i].AddStop(2);
            }

            // Act
            await elevatorService.MoveElevatorsAsync();

            // Assert
            foreach(var elevator in building.Elevators)
            {
                Assert.Equal(2, elevator.CurrentFloor);
                Assert.Equal(Direction.Up, elevator.CurrentDirection);
                Assert.Equal(State.Idle, elevator.CurrentState);  
            }
        }

        [Fact]
        public async Task MoveElevatorsAsync_WithNoStops_ElevatorsShouldRemainIdle()
        {
            // Arrange
            InitializeTestEnvironment(10, 4);

            // Act
            await elevatorService.MoveElevatorsAsync();

            // Assert
            foreach (var elevator in building.Elevators)
            {
                Assert.Equal(1, elevator.CurrentFloor);
                Assert.Equal(Direction.Up, elevator.CurrentDirection);
                Assert.Equal(State.Idle, elevator.CurrentState);
            }
        }

        [Fact]
        public async Task MoveElevatorsAsync_WithStopsInDifferentDirections_ElevatorsShouldMoveAccordingly()
        {
            // Arrange
            InitializeTestEnvironment(10, 4);

            var car1 = building.Elevators[0];
            car1.AddStop(4);
            car1.Update(2, Direction.Up, State.Moving, car1.Label);
            
            var car2 = building.Elevators[1];
            car2.AddStop(2);
            car2.Update(3, Direction.Down, State.Moving, car2.Label);

            var car3 = building.Elevators[2];

            // Act
            await elevatorService.MoveElevatorsAsync();

            // Assert
            Assert.Equal(3, car1.CurrentFloor);
            Assert.Equal(Direction.Up, car1.CurrentDirection);
            Assert.Equal(State.Moving, car1.CurrentState);

            Assert.Equal(2, car2.CurrentFloor);
            Assert.Equal(Direction.Down, car2.CurrentDirection);
            Assert.Equal(State.Idle, car2.CurrentState);

            Assert.Equal(1, car3.CurrentFloor);
            Assert.Equal(Direction.Up, car3.CurrentDirection);
            Assert.Equal(State.Idle, car3.CurrentState);
        }

        [Fact]
        public async Task MoveElevatorsAsync_AtTopFloor_ShouldStayAtTopFloor()
        {
            // Arrange
            InitializeTestEnvironment(10, 4);

            var topFloorElevator = building.Elevators[0];
            topFloorElevator.Update(10, Direction.Up, State.Idle, topFloorElevator.Label);
            topFloorElevator.AddStop(building.TotalFloors); 

            // Act
            await elevatorService.MoveElevatorsAsync();

            // Assert
            int expectedFloor = building.TotalFloors; 
            Assert.Equal(expectedFloor, topFloorElevator.CurrentFloor);
        }

        [Fact]
        public async Task MoveElevatorsAsync_AtGroundFloor_ShouldStayAtGroundFloor()
        {
            // Arrange
            InitializeTestEnvironment(10, 4);

            var groundFloorElevator = building.Elevators[0];

            // Act
            await elevatorService.MoveElevatorsAsync();

            // Assert
            Assert.Equal(1, groundFloorElevator.CurrentFloor);
        }

        #endregion

        #region GetElevatorStatus

        [Fact]
        public async Task GetElevatorStatus_ShouldReturnCorrectStatus()
        {
            //Arrange
            InitializeTestEnvironment(10, 4);

            var car1 = building.Elevators[0];
            var car1Label = ELEVATOR_IDENTIFIER + " " + 1;
            car1.Update(5, Direction.Up, State.Moving, car1Label);

            var car2 = building.Elevators[1];
            var car2Label = ELEVATOR_IDENTIFIER + " " + 2;
            car2.Update(3, Direction.Up, State.Idle, car2Label);

            //Expected Output
            var expectedStatus = new StringBuilder();
            expectedStatus.AppendLine("car 1 is on floor 5 - Moving - Up");
            expectedStatus.AppendLine("car 2 is on floor 3 - Idle");
            expectedStatus.AppendLine("car 3 is on floor 1 - Idle");
            expectedStatus.AppendLine("car 4 is on floor 1 - Idle");

            //Act
            var actualStatus = await elevatorService.GetElevatorStatus();

            //Assert
            Assert.Equal(expectedStatus.ToString(), actualStatus);

        }

        #endregion

    }
}