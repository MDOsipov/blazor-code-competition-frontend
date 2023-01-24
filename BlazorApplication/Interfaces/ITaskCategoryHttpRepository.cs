using BlazorApplication.Features;
using BlazorApplication.Models;
using Task = System.Threading.Tasks.Task;

namespace BlazorApplication.Interfaces
{
    public interface ITaskCategoryHttpRepository
    {
        Task<PagingResponse<TaskCategory>> GetTaskCategory(TaskCategoryParameters taskCategoryParameters);
        Task CreateTaskCategory(TaskCategory taskCategory);
        Task<TaskCategory> GetTaskCategoryById(string id);
        Task UpdateTaskCategory(TaskCategory taskCategory);
        Task DeleteTaskCategory(int id);
    }
}
