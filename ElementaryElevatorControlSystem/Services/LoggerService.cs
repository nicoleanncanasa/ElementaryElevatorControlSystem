using ElementaryElevatorControlSystem.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementaryElevatorControlSystem.Services
{
    public class LoggerService : ILoggerService
    {
        public async Task LogInfoAsync(string message)
        {
            await Task.Run(() => Console.WriteLine(message));
        }

        public async Task LogDebugAsync(string message)
        {
            await Task.Run(() => Console.WriteLine($"DEBUG: {message}"));
        }

        public async Task LogErrorAsync(string message)
        {
            await Task.Run(() => Console.WriteLine($"ERROR: {message}"));
        }
    }
}
