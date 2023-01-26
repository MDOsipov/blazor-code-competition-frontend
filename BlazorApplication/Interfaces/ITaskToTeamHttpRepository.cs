using BlazorApplication.Models;

namespace BlazorApplication.Interfaces
{
	public interface ITaskToTeamHttpRepository
	{
		System.Threading.Tasks.Task AddTaskToTeam(TaskToTeam taskToTeam);
		System.Threading.Tasks.Task DeleteTeamTaskByTaskIdAndTeamId(string taskId, string teamId);
		System.Threading.Tasks.Task SubmitTask(string taskId, string teamId, SubmitTaskDataDto submitTaskDataDto);

	}
}
