using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using BlazorApplication.Shared;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using System.Text.Json;
using Task = System.Threading.Tasks.Task;

namespace BlazorApplication.Pages
{
	public partial class EvaluateTask
	{
		private SuccessfulSubmit? _notification;

		[Inject]
		public ITaskToTeamHttpRepository? TaskToTeamRepo { get; set; }

		[Inject]
		public ILogger<SubmitTask> Logger { get; set; }
		[Parameter]
		public string taskIdStr { get; set; } = "";
		[Parameter]
		public string teamIdStr { get; set; } = "";
		[Parameter]
		public string competitionIdStr { get; set; } = "";

		private TaskToTeam TaskToTeam = new TaskToTeam();

		private ErrorBoundary? errorBoundary;
		public EvaluateTaskDataDto evaluateTaskData { get; set; } = new EvaluateTaskDataDto();
		private Enums.TaskStatus taskStatus { get; set; } = Enums.TaskStatus.Success;

		protected async override Task OnInitializedAsync()
		{
			await GetTaskToTeam();
		}

		private async System.Threading.Tasks.Task GetTaskToTeam()
		{
			Logger.LogInformation("Get task to team Http repository method is called");
			try
			{
				if (taskIdStr != "" && teamIdStr != "" && competitionIdStr != "")
				{
					TaskToTeam = await TaskToTeamRepo.GetTaskToTeamByTaskIdAndTeamIdAndCompetitionId(taskIdStr, teamIdStr, competitionIdStr);
				}
			}
			catch (Exception ex)
			{
				Logger.LogError($"Error: {ex}");
				throw new Exception("Oops! Something went wrong while getting a task to team info!", ex);
			}
		}

		private async System.Threading.Tasks.Task Evaluate()
		{

			Logger.LogInformation("Evaluate method is called");
			if (_notification is not null && TaskToTeamRepo is not null)
			{
				try
				{
					if (taskIdStr != "" && teamIdStr != "" && competitionIdStr != "")
					{
						evaluateTaskData.TaskId = Int32.Parse(taskIdStr);
						evaluateTaskData.TeamId = Int32.Parse(teamIdStr);
						evaluateTaskData.CompetitionId = Int32.Parse(competitionIdStr);
						evaluateTaskData.TaskStatusId = (int)taskStatus;
					}
					else
					{
						Logger.LogError($"Error: component parameters are not valid!");
						throw new Exception("Error: component parameters are not valid!");
					}

					// Logger.LogInformation($"We would send: {JsonSerializer.Serialize(evaluateTaskData)}");
					await TaskToTeamRepo.EvaluateTask(evaluateTaskData);
					Logger.LogInformation($"Success. The task is successfully evaluated");
					_notification.Show();
				}
				catch (Exception ex)
				{
					Logger.LogError($"Error: {ex}");
					throw new Exception("Oops! Something went wrong while evaluating a task!", ex);
				}
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
