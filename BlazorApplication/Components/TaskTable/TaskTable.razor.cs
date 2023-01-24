using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static System.Net.WebRequestMethods;
using System.Text.Json;
using System.Text;
using Task = System.Threading.Tasks.Task;

namespace BlazorApplication.Components.TaskTable
{
	public partial class TaskTable
	{
		public bool ChangeMode { get; set; } = false;
		[Inject]
		public IJSRuntime Js { get; set; }

		[Parameter]
		public List<Models.Task> Tasks { get; set; }

		public List<Models.Task> AllPossibleTasks { get; set; }

		[Parameter]
		public bool isTaskToCompetitionFlag { get; set; } = false;

		[Parameter]
		public EventCallback<int> OnDeleted { get; set; }

		[Inject]
		public NavigationManager NavigationManager { get; set; }

		[Inject]
		public ITaskHttpRepository TaskRepo { get; set; }


		protected async override Task OnInitializedAsync()
		{
			TaskParameters taskParameters = new TaskParameters()
			{
				switchOff = true
			};
			var pagingResponse = await TaskRepo.GetTasks(taskParameters);
			AllPossibleTasks = pagingResponse.Items;
		}


		private void RedirectToUpdate(int id)
		{
			var url = Path.Combine("/updateTask/", id.ToString());
			Console.WriteLine("Url - " + url);
			NavigationManager.NavigateTo(url);
		}

		private async Task Delete(int id)
		{
			var task = Tasks.FirstOrDefault(p => p.Id.Equals(id));
			var confirmed = await Js.InvokeAsync<bool>("confirm", $"Are you sure you want to delete {task.TaskName} task?");

			if(confirmed) 
			{
				await OnDeleted.InvokeAsync(id);
			}
		}

		public async void ChangeModeFun()
		{
			ChangeMode = !ChangeMode;
		}
	}
}
