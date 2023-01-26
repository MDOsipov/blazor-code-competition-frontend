using BlazorApplication.Features;

namespace BlazorApplication.Interfaces
{
    public interface ITaskHttpRepository
    {
        Task<PagingResponse<Models.Task>> GetTasks(TaskParameters taskParameters);
		Task<PagingResponse<Models.Task>> GetTasksByCompetitionId(TaskParameters taskParameters, string id);
        Task<PagingResponse<Models.TaskWithTimesDto>> GetTasksByTeamId(TaskParameters taskParameters, string teamId);
        Task<PagingResponse<Models.TaskWithTimesDto>> GetSubmittedTasksByTeamId(TaskParameters taskParameters, string teamId);
        Task CreateTask(Models.Task task);
        Task<Models.Task> GetTaskById(string id);
        Task UpdateTask(Models.Task task);
        Task DeleteProduct(int id);
    }
}
