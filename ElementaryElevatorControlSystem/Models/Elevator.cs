using ElementaryElevatorControlSystem.Enums;

namespace ElementaryElevatorControlSystem.Models
{
    public class Elevator
    {
        #region Fields
        private List<int> Stops;
        //Time duration for car movement between floors
        private const int CAR_MOVEMENT_DURATION = 10000;
        //Time duration for passengers to enter/leave
        private const int BOARDING_TIME_DURATION = 10000;
        #endregion

        #region Properties
        public Guid Id { get; private set; }
        public string Label { get; private set; }
        public int CurrentFloor { get; private set; }
        public Direction CurrentDirection { get; private set; }
        public State CurrentState {  get; private set; }
        public IReadOnlyList<int> ReadOnlyStops => Stops.AsReadOnly();
        #endregion

        #region Methods
        public Elevator() 
        {
            Id = Guid.NewGuid();
            CurrentFloor = 1; //Assuming 1st floor as the start
            CurrentDirection = Direction.Up;
            CurrentState = State.Idle;
            Label = "";
            Stops = new List<int>();
        }

        public Elevator(int floor, Direction direction, State state, string label)
        {
            Id = Guid.NewGuid();
            CurrentFloor = floor;
            CurrentDirection = direction;
            CurrentState = state;
            Label = label;
            Stops = new List<int>();
        }

        public void Update(int floor, Direction direction, State state, string label)
        {
            CurrentFloor = floor;
            CurrentDirection = direction;
            CurrentState = state;
            Label = label;
        }

        /// <summary>
        /// Add a stop floor to the elevator's list of stops if it's not already included.
        /// </summary>
        /// <param name="floor"></param>
        public void AddStop(int floor)
        {
            if (!Stops.Contains(floor))
            {
                Stops.Add(floor);
                Stops.Sort();
            }
        }

        /// <summary>
        /// Move the elevator to the next floor based on its current stops and direction.
        /// </summary>
        /// <param name="highestFloor">Maximum floor of the building</param>
        /// <returns></returns>
        public async Task MoveToNextFloorAsync(int highestFloor)
        {
            if (Stops.Count == 0)
            {
                CurrentState = State.Idle;
                return; 
            }

            // If the elevator has not reached the stop yet, move towards it
            CurrentState = State.Moving;

            if (CurrentFloor < Stops.First())
            {
                CurrentFloor++;
                CurrentDirection = Direction.Up;
                await Task.Delay(CAR_MOVEMENT_DURATION); //Simulating car movement up
                if (CurrentFloor == highestFloor)
                {
                    CurrentDirection = Direction.Down;
                }
            }
            else if (CurrentFloor > Stops.First())
            {
                CurrentFloor--;
                CurrentDirection = Direction.Down;
                await Task.Delay(CAR_MOVEMENT_DURATION); //Simulating car movement down
            }

            if (CurrentFloor == Stops.First())
            {
                // Elevator has reached the stop
                Stops.RemoveAt(0); // Remove the stop
                await Task.Delay(BOARDING_TIME_DURATION); // Simulating boarding time
                CurrentState = Stops.Count > 0 ? State.Moving : State.Idle;
                return;
            }
        }

        #endregion
    }
}
