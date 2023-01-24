using BlazorApplication.Models;
using Task = System.Threading.Tasks.Task;

namespace BlazorApplication.Interfaces
{
    public interface ITaskToCompetitionRepository
    {
        Task AddTaskToCompetition(TaskToCompetition taskToCompetition);
        Task DeleteTaskToCompetition(int taskId, int competitionId);
    }
}
