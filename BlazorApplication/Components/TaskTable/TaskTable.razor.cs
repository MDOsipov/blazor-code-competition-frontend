using Microsoft.AspNetCore.Components;

namespace BlazorApplication.Components.TaskTable
{
	public partial class TaskTable
	{
		[Parameter]
		public List<Models.Task> Tasks { get; set; }
	}
}
