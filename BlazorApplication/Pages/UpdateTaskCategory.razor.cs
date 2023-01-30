using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using BlazorApplication.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Text.Json;

namespace BlazorApplication.Pages
{
    public partial class UpdateTaskCategory
    {
        private TaskCategory _taskCategory = new TaskCategory();
        private SuccessNotification _notification;
        private ErrorBoundary? errorBoundary;
        public List<Team> teamList { get; set; } = new List<Team>();

        [Inject]
        public ITaskCategoryHttpRepository TaskCategoryRepo { get; set; }

        [Inject]
        public ILogger<UpdateTaskCategory> Logger { get; set; }

        [Parameter]
        public string Id { get; set; } = "";

        protected async override System.Threading.Tasks.Task OnInitializedAsync()
        {
            await GetTaskCategory();
        }

        private async System.Threading.Tasks.Task GetTaskCategory()
        {
            Logger.LogInformation("Get task category method is called");
            try
            {
                _taskCategory = await TaskCategoryRepo.GetTaskCategoryById(Id);
                Logger.LogInformation($"Success. Task category: {JsonSerializer.Serialize(_taskCategory)}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while deleting participants!", ex);
            }
        }

        private async System.Threading.Tasks.Task Update()
        {
            Logger.LogInformation("Update method is called");

            try
            {
                await TaskCategoryRepo.UpdateTaskCategory(_taskCategory);
                Logger.LogInformation($"Success. The task category is updated");
                _notification.Show();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while updating the task category!", ex);
            }
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
