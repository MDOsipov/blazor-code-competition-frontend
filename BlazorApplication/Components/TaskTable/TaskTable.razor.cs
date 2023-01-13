using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorApplication.Components.TaskTable
{
	public partial class TaskTable
	{
		[Inject]
		public IJSRuntime Js { get; set; }

		[Parameter]
		public List<Models.Task> Tasks { get; set; }

		[Parameter]
		public EventCallback<int> OnDeleted { get; set; }

		[Inject]
		public NavigationManager NavigationManager { get; set; }	

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
	}
}
