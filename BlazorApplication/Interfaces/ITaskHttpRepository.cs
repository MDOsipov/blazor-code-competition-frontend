using BlazorApplication.Features;

namespace BlazorApplication.Interfaces
{
    public interface ITaskHttpRepository
    {
        Task<PagingResponse<Models.Task>> GetTasks(TaskParameters taskParameters);
		Task<PagingResponse<Models.Task>> GetTasksByCompetitionId(TaskParameters taskParameters, string id);
		Task CreateTask(Models.Task task);
        Task<Models.Task> GetTaskById(string id);
        Task UpdateTask(Models.Task task);
        Task DeleteProduct(int id);
    }
}
