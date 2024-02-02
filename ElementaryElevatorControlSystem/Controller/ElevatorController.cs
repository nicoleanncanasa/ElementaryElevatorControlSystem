using ElementaryElevatorControlSystem.Enums;
using ElementaryElevatorControlSystem.Services;
using ElementaryElevatorControlSystem.Services.Interfaces;
using System.Drawing;

namespace ElementaryElevatorControlSystem.Controllers
{
    public class ElevatorController
    {
        private readonly IElevatorService _elevatorService;
        private readonly ILoggerService _loggerService;

        public ElevatorController(IElevatorService elevatorService, ILoggerService loggerService)
        {
            _elevatorService = elevatorService;
            _loggerService = loggerService;
        }

        /// <summary>
        /// Request an elevator to a specific floor with a given direction
        /// </summary>
        /// <param name="floor">Requested floor</param>
        /// <param name="direction">Requested direction</param>
        public async Task RequestElevatorAsync(int floor, Direction direction)
        {
            try
            {              
                await _elevatorService.CallElevatorAsync(floor, direction);
                await _loggerService.LogInfoAsync(direction.ToString() + " request on floor " + floor + " received");
            }
            catch (Exception ex)
            {
                await _loggerService.LogErrorAsync($"Error in requesting elevator: {ex.Message}");
                throw; 
            }
        }

        /// <summary>
        /// Move all elevators to their next destination floors
        /// </summary>
        public async Task MoveElevatorsAsync()
        {
            try
            {
                await _elevatorService.MoveElevatorsAsync();
            }
            catch (Exception ex)
            {
                await _loggerService.LogErrorAsync($"Error while elevators are moving: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Display the status of all elevators in the building
        /// </summary>
        public async Task DisplayStatusAsync()
        {
            try
            {
                var status = await _elevatorService.GetElevatorStatus();
                await _loggerService.LogInfoAsync(status);
            }
            catch (Exception ex)
            {
                await _loggerService.LogErrorAsync($"Error in displaying status: {ex.Message}");
                throw;
            }
        }
    }

}
