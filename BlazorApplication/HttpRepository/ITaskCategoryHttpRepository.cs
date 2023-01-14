using BlazorApplication.Models;

namespace BlazorApplication.HttpRepository
{
    public interface ITaskCategoryHttpRepository
    {
        Task<List<TaskCategory>> GetTaskCategory();
    }
}
