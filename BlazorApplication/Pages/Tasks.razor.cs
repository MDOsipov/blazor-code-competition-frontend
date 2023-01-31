using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Text.Json;

namespace BlazorApplication.Pages
{
    public partial class Tasks
	{
		public List<Models.Task> TaskList { get; set; } = new List<Models.Task>();
		public MetaData MetaData { get; set; } = new MetaData();
		private TaskParameters _taskParameters = new TaskParameters();
        private ErrorBoundary? errorBoundary;

        [Inject]
		public ITaskHttpRepository TaskRepo { get; set; }

        [Parameter]
        public bool successResponse { get; set; } = false;

		[Inject]
		public ILogger<Tasks> Logger { get; set; }

		protected async override System.Threading.Tasks.Task OnInitializedAsync()
		{
			await GetTasks();
		}

		private async System.Threading.Tasks.Task SelectedPage(int page)
		{
			_taskParameters.PageNumber = page;
			await GetTasks();
		}

		protected async System.Threading.Tasks.Task GetTasks()
		{
            Logger.LogInformation("Get tasks method is called");
            try
            {
                var pagingResponse = await TaskRepo.GetTasks(_taskParameters);
                TaskList = pagingResponse.Items;
                MetaData = pagingResponse.MetaData;
                successResponse = true;
                Logger.LogInformation($"Success. Tasks: {JsonSerializer.Serialize(TaskList)}");
            }
            catch (Exception ex)
			{
                Logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while getting a list of tasks!", ex);
            }
        }

		private async System.Threading.Tasks.Task DeleteTask(int id)
		{
            Logger.LogInformation("Delete task method is called");
            try
            {
                await TaskRepo.DeleteTask(id);
                _taskParameters.PageNumber = 1;
                Logger.LogInformation($"Success. The task is deleted");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while deleting a task!", ex);
            }
            await GetTasks();
        }

        private async System.Threading.Tasks.Task SearchChanged(string searchString)
		{
			_taskParameters.PageNumber = 1;
			_taskParameters.SearchString = searchString;
			await GetTasks();
		}

		private async System.Threading.Tasks.Task SortChanged(string orderBy)
		{
			_taskParameters.OrderBy = orderBy;
			await GetTasks();
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
