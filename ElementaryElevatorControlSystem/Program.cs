using ElementaryElevatorControlSystem.Controllers;
using ElementaryElevatorControlSystem.Enums;
using ElementaryElevatorControlSystem.Models;
using ElementaryElevatorControlSystem.Services;
using ElementaryElevatorControlSystem.Services.Interfaces;


#region Initial Setup and Sample Parameters

int numberOfElevators = 4;
int totalFloors = 10;

Building building = new Building(totalFloors, numberOfElevators);
IElevatorService elevatorService = new ElevatorService(building);
ILoggerService loggerService = new LoggerService();
ElevatorController controller = new ElevatorController(elevatorService, loggerService);

Random rnd = new Random();
#endregion

while (true)
{
    try
    {
        int floor = rnd.Next(1, totalFloors + 1);
        Direction direction = rnd.Next(0, 2) == 0 ? Direction.Up : Direction.Down;

        await controller.RequestElevatorAsync(floor, direction);
        await controller.MoveElevatorsAsync();
        await controller.DisplayStatusAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }
   
}