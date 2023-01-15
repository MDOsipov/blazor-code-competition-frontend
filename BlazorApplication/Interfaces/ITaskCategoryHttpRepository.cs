using BlazorApplication.Models;

namespace BlazorApplication.Interfaces
{
    public interface ITaskCategoryHttpRepository
    {
        Task<List<TaskCategory>> GetTaskCategory();
    }
}
