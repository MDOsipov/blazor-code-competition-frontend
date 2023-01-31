using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using BlazorApplication.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorApplication.Pages
{
    public partial class CreateTaskCategory
    {
        private TaskCategory _taskCategory = new TaskCategory();
        private SuccessNotification _notification;
        private ErrorBoundary? errorBoundary;

        [Inject]
        public ITaskCategoryHttpRepository TaskCategoryRepo { get; set; }

        [Inject]
        public ILogger<CreateTaskCategory> Logger { get; set; }
        protected async override System.Threading.Tasks.Task OnInitializedAsync()
        {
          
        }

        private async void Create()
        {
            Logger.LogInformation("Create method is called");
            try
            {
                await TaskCategoryRepo.CreateTaskCategory(_taskCategory);
                Logger.LogInformation($"Success. A new task category is created");
                _notification.Show();
            }
            catch(Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while creating a new task category!", ex);
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
