using BlazorApplication.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace BlazorApplication.Components.TaskTable
{
	public partial class TaskTableAllSubmitted
	{
		[Parameter]
		public List<Models.SubmittedTaskDto> Tasks { get; set; }

		[Parameter]
		public int CompetitionId { get; set; } = 0;

		[Parameter]
		public int TeamId { get; set; } = 0;

		[Inject]
		public ILogger<TaskTableAllSubmitted> logger { get; set; }	

		[Inject]
		public NavigationManager NavigationManager { get; set; }

		public Dictionary<int, string> StatusDict = new Dictionary<int, string>()
		{
			[(int)Enums.TaskStatus.Success] = "Success",
			[(int)Enums.TaskStatus.Fail] = "Fail",
			[(int)Enums.TaskStatus.PartialSuccess] = "Partial success",
			[(int)Enums.TaskStatus.Submitted] = "Submitted"
		};

		private void RedirectToUpdate(int taskId)
		{
			logger.LogInformation($"Redirect to update method is called with parameters: taskId: {taskId}");
			var url = Path.Combine("/evaluateTask/", taskId.ToString(), TeamId.ToString(), CompetitionId.ToString());
			logger.LogInformation($"Url - {url}");
			NavigationManager.NavigateTo(url);
		}
	}
}

