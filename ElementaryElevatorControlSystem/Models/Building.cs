using ElementaryElevatorControlSystem.Enums;

namespace ElementaryElevatorControlSystem.Models
{
    public class Building
    {
        public const int GROUND_FLOOR = 1;
        public const string ELEVATOR_IDENTIFIER = "car";
        public Guid Id { get; set; }
        public int TotalFloors { get; private set; }
        public List<Elevator> Elevators { get; private set; }

        public Building(int totalFloors, int numberOfElevators)
        {
            Id = Guid.NewGuid();
            TotalFloors = totalFloors;
            Elevators = new List<Elevator>();
            SetupElevators(numberOfElevators);
        }

        private void SetupElevators(int numberOfElevators)
        {
            for (int i = 1; i <= numberOfElevators; i++)
            {
                var elevator = new Elevator(GROUND_FLOOR, Direction.Up, State.Idle, ELEVATOR_IDENTIFIER + " " + i);
                Elevators.Add(elevator);
            }
        }
    }
}
