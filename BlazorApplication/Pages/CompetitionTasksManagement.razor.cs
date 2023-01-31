using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorApplication.Pages
{
    public partial class CompetitionTasksManagement
    {
		
		[Parameter]
        public string id { get; set; } = string.Empty;

		[Parameter]
		public bool successResponse { get; set; }

		[Inject]
		public ITaskToCompetitionRepository taskToCompetitionRepo { get; set; }
			
		public List<Models.Task> TaskList { get; set; } = new List<Models.Task>();
		public MetaData MetaData { get; set; } = new MetaData();
		private TaskParameters _taskParameters = new TaskParameters();
        private ErrorBoundary? errorBoundary;

        [Inject]
		public ITaskHttpRepository TaskRepo { get; set; }
		[Inject]
		public ILogger<CompetitionTasksManagement> Logger { get; set; }	

		protected async override System.Threading.Tasks.Task OnInitializedAsync()
		{
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
                var pagingResponse = await TaskRepo.GetTasksByCompetitionId(_taskParameters, id);
                TaskList = pagingResponse.Items;
                MetaData = pagingResponse.MetaData;
                successResponse = true;
                Logger.LogInformation($"Success. Tasks: {JsonSerializer.Serialize(TaskList)}");
            }
            catch (Exception ex)
			{
                Logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while getting a list of tasks for this competition!", ex);
            }
        }

		private async System.Threading.Tasks.Task DeleteTask(int taskId)
		{
            Logger.LogInformation("Delete task method is called");
            try
            {
                await taskToCompetitionRepo.DeleteTaskToCompetition(taskId, Int32.Parse(id));
                _taskParameters.PageNumber = 1;
                Logger.LogInformation($"Success. Task is deleted");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while deleting a task for this competition!", ex);
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
	}
}
