using ElementaryElevatorControlSystem.Enums;
using ElementaryElevatorControlSystem.Models;
using ElementaryElevatorControlSystem.Services.Interfaces;
using System.Text;

public class ElevatorService : IElevatorService
{
    private readonly Building _building;
    public ElevatorService(Building building)
    {
        _building = building;
    }

    /// <summary>
    /// Call an elevator to a specific floor with a given direction
    /// </summary>
    /// <param name="floor">Requested floor</param>
    /// <param name="direction">Requested direction</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task CallElevatorAsync(int floor, Direction direction)
    {
        //Validate floor input
        if (floor < 1 || floor > _building.TotalFloors)
        {
            throw new ArgumentOutOfRangeException(nameof(floor), "Floor out of range.");
        }

        // Selecting the best elevator considering direction and distance
        var bestElevator = _building.Elevators
            .Select(elevator => new
            {
                Elevator = elevator,
                Distance = Math.Abs(elevator.CurrentFloor - floor),
                IsMovingTowards = (elevator.CurrentDirection == Direction.Up && floor > elevator.CurrentFloor) ||
                                  (elevator.CurrentDirection == Direction.Down && floor < elevator.CurrentFloor)
            })
            .OrderBy(e => e.IsMovingTowards ? 0 : 1) 
            .ThenBy(e => e.Distance) 
            .FirstOrDefault()?.Elevator;

        if (bestElevator != null)
        {
            bestElevator.AddStop(floor);
        }
        else
        {
            throw new InvalidOperationException("No suitable elevator found.");
        }
    }

    /// <summary>
    /// Move all elevators to their next destination floors
    /// </summary>
    /// <returns></returns>
    public async Task MoveElevatorsAsync()
    {
        // Move each elevator asynchronously using Task.Run
        var tasks = _building.Elevators.Select(elevator => Task.Run(() => elevator.MoveToNextFloorAsync(_building.TotalFloors)));
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Get the status of all elevators in the building
    /// </summary>
    /// <returns>Status of all elevator</returns>
    public async Task<string> GetElevatorStatus()
    {
        // Create a status report with elevator information
        var status = new StringBuilder();
        for (int i = 0; i < _building.Elevators.Count; i++)
        {
            var state = _building.Elevators[i].CurrentState == State.Idle ? _building.Elevators[i].CurrentState.ToString() :
                        $"{_building.Elevators[i].CurrentState.ToString()} - {_building.Elevators[i].CurrentDirection.ToString()}";
            status.AppendLine($"{_building.Elevators[i].Label} is on floor {_building.Elevators[i].CurrentFloor} - {state}");
        }
        return status.ToString();
    }
}
