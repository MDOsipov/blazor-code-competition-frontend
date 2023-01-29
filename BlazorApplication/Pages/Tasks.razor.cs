using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

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
			try
			{
                var pagingResponse = await TaskRepo.GetTasks(_taskParameters);
                TaskList = pagingResponse.Items;
                MetaData = pagingResponse.MetaData;
            }
			catch(Exception ex)
			{
                throw new System.Exception("Oops! Something went wrong while getting a list of tasks!", ex);
            }
        }

		private async System.Threading.Tasks.Task DeleteTask(int id)
		{
			try
			{
                await TaskRepo.DeleteTask(id);
                _taskParameters.PageNumber = 1;
            }
            catch (Exception ex)
            {
                throw new System.Exception("Oops! Something went wrong while deleting a task!", ex);
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
