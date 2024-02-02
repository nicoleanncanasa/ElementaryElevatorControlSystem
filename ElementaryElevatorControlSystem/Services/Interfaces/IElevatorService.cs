using ElementaryElevatorControlSystem.Enums;

namespace ElementaryElevatorControlSystem.Services.Interfaces
{
    public interface IElevatorService
    {
        Task CallElevatorAsync(int floor, Direction direction);
        Task MoveElevatorsAsync();
        Task<string> GetElevatorStatus();

    }
}
