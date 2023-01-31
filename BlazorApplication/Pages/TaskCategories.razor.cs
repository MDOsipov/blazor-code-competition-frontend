using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Text.Json;

namespace BlazorApplication.Pages
{
    public partial class TaskCategories
    {
        public List<Models.TaskCategory> TaskCategoryList { get; set; } = new List<Models.TaskCategory>();
        public MetaData MetaData { get; set; } = new MetaData();
        private TaskCategoryParameters _taskCategoryParameters = new TaskCategoryParameters();
        private ErrorBoundary? errorBoundary;

        [Inject]
        public ITaskCategoryHttpRepository TaskCategoryRepo { get; set; }

        [Inject]
        public ILogger<TaskCategories> Logger { get; set; }

        [Parameter]
        public bool successResponse { get; set; } = false;
        protected async override System.Threading.Tasks.Task OnInitializedAsync()
        {
            await GetTaskСategories();
        }

        private async System.Threading.Tasks.Task SelectedPage(int page)
        {
            _taskCategoryParameters.PageNumber = page;
            await GetTaskСategories();
        }

        protected async System.Threading.Tasks.Task GetTaskСategories()
        {
            Logger.LogInformation("Get task categories method is called");
            try
            {
                var pagingResponse = await TaskCategoryRepo.GetTaskCategory(_taskCategoryParameters);
                TaskCategoryList = pagingResponse.Items;
                MetaData = pagingResponse.MetaData;
                successResponse = true;
                Logger.LogInformation($"Success. Task categories: {JsonSerializer.Serialize(TaskCategoryList)}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while getting a list of task categories!", ex);
            }
        }

        
        private async System.Threading.Tasks.Task DeleteTaskCategory(int id)
        {
            Logger.LogInformation("Delete task category method is called");
            try
            {
                await TaskCategoryRepo.DeleteTaskCategory(id);
                _taskCategoryParameters.PageNumber = 1;
                Logger.LogInformation($"Success. The task category is deleted");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while deleting a task category!", ex);
            }
            await GetTaskСategories();
        }
        protected override void OnParametersSet()
        {
            errorBoundary?.Recover();
        }
        private void ResetError()
        {
            errorBoundary?.Recover();
        }
    }
}
