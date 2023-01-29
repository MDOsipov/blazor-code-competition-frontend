using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using Microsoft.AspNetCore.Components;
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

		[Inject]
		public ITaskHttpRepository TaskRepo { get; set; }

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
			var pagingResponse = await TaskRepo.GetTasksByCompetitionId(_taskParameters, id);
			TaskList = pagingResponse.Items;
			MetaData = pagingResponse.MetaData;
			successResponse = pagingResponse.SuccessRequest;
		}

		private async System.Threading.Tasks.Task DeleteTask(int taskId)
		{
			await taskToCompetitionRepo.DeleteTaskToCompetition(taskId, Int32.Parse(id));
			_taskParameters.PageNumber = 1;
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
