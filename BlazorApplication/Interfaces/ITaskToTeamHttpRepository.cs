using BlazorApplication.Models;

namespace BlazorApplication.Interfaces
{
	public interface ITaskToTeamHttpRepository
	{
		Task<TaskToTeam> GetTaskToTeamByTaskIdAndTeamIdAndCompetitionId(string taskId, string teamId, string competitionId);
		Task<int> GetOverallScoreByTeamIdAndCompetitionId(string teamId, string competitionId);
		System.Threading.Tasks.Task AddTaskToTeam(TaskToTeam taskToTeam);
		System.Threading.Tasks.Task DeleteTeamTaskByTaskIdAndTeamId(string taskId, string teamId);
		System.Threading.Tasks.Task SubmitTask(string taskId, string teamId, SubmitTaskDataDto submitTaskDataDto);
		System.Threading.Tasks.Task EvaluateTask(EvaluateTaskDataDto evaluateTaskDataDto);
    }
}
