using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class WorkoutHub : Hub
{
    public async Task UpdateWorkoutDay(int userId, int dayIndex, bool hasWorkout)
    {
        await Clients.All.SendAsync("ReceiveWorkoutUpdate", userId, dayIndex, hasWorkout);
    }
}
