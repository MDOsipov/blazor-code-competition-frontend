using BlazorApplication.Features;
using BlazorApplication.HttpRepository;
using BlazorApplication.Models;
using Microsoft.AspNetCore.Components;

namespace BlazorApplication.Pages
{
	public partial class Tasks
	{
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
			var pagingResponse = await TaskRepo.GetTasks(_taskParameters);
			TaskList = pagingResponse.Items;
			MetaData = pagingResponse.MetaData;
		}
	}
}
