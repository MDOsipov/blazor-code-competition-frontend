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

        [Parameter]
        public string Id { get; set; } = "";

        protected async override System.Threading.Tasks.Task OnInitializedAsync()
        {
            await GetTaskCategory();
        }

        private async System.Threading.Tasks.Task GetTaskCategory()
        {
            try 
            {
                _taskCategory = await TaskCategoryRepo.GetTaskCategoryById(Id);
                Console.WriteLine("Got task category object: ");
                Console.WriteLine(JsonSerializer.Serialize(_taskCategory));
            }
            catch(Exception ex)
            {
                throw new System.Exception("Oops! Something went wrong while deleting participants!", ex);
            }
        }

        private async System.Threading.Tasks.Task Update()
        {
            try
            {
                await TaskCategoryRepo.UpdateTaskCategory(_taskCategory);
                _notification.Show();
            }
            catch (Exception ex)
            {
                throw new System.Exception("Oops! Something went wrong while updating the task category!", ex);
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
