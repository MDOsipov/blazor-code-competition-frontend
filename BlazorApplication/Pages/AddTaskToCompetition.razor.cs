using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using BlazorApplication.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;
using Task = BlazorApplication.Models.Task;

namespace BlazorApplication.Pages
{
	public partial class AddTaskToCompetition
	{
		[Parameter]
        public string competitionIdStr { get; set; } = "";
        private ErrorBoundary? errorBoundary;
        protected override void OnParametersSet()
        {
            errorBoundary?.Recover();
        }
        private void ResetError()
        {
            errorBoundary?.Recover();
        }
        public string navUrlToSend { get; set; } = "";
        public int competitionId { get; set; } = 0;
        private SuccessNotification? _notification;
		public int newTaskId { get; set; } = 0;

        [Inject]
		public ITaskHttpRepository TaskRepo { get; set; }
        [Inject]
        public ILogger<AddTaskToCompetition> Logger { get; set; }   

        [Inject]
        public ITaskToCompetitionRepository TaskToCompetitionRepo { get; set; }

        private TaskParameters _taskParameters = new TaskParameters()
		{
			switchOff = true
		};
		public List<Task> TaskList { get; set; } = new List<Task>();
		public List<Task> CurrentTaskList { get; set; } = new List<Task>();


		protected async override System.Threading.Tasks.Task OnInitializedAsync()
		{
			if (competitionIdStr != "")
			{
				competitionId = Int32.Parse(competitionIdStr);
				navUrlToSend = "/competitionTasksManagement/" + competitionIdStr.ToString();
            }
            await GetCurrentTaskList();
			await GetTasks();
		}

		protected async System.Threading.Tasks.Task GetCurrentTaskList()
		{
			Logger.LogInformation("Get current task list method is called");
			try
			{
				var pagingResponse = await TaskRepo.GetTasksByCompetitionId(_taskParameters, competitionIdStr);
				CurrentTaskList = pagingResponse.Items;
				Logger.LogInformation($"Success. Current task list: {JsonSerializer.Serialize(CurrentTaskList)}");
			}
			catch (Exception ex)
			{
				Logger.LogError($"Error: {ex}");
				throw new System.Exception("Oops! Something went wrong while getting a list of tasks!", ex);
			}
		}

		protected async System.Threading.Tasks.Task GetTasks()
		{
            Logger.LogInformation("Get tasks method is called");
            try
            {
                var pagingResponse = await TaskRepo.GetTasks(_taskParameters);
                TaskList = pagingResponse.Items;
                TaskList = TaskList.Where(tl => CurrentTaskList.Select(ctl => ctl.Id).ToList().IndexOf(tl.Id) == -1).ToList();
                newTaskId = TaskList.FirstOrDefault().Id;
                Logger.LogInformation($"Success. Task list: {JsonSerializer.Serialize(TaskList)}");
            }
            catch (Exception ex)
			{
                Logger.LogError($"Error: {ex}");
                throw new System.Exception("Oops! Something went wrong while getting a list of tasks!", ex);
            }
        }

        private async void Create()
        {
            Logger.LogInformation("Create method is called");
            TaskToCompetition taskToCompetition = new TaskToCompetition()
			{
				CompetitionId = competitionId,
				TaskId = newTaskId
			};

            try 
            {
                await TaskToCompetitionRepo.AddTaskToCompetition(taskToCompetition);
                Logger.LogInformation($"Success. New task to competition is added");
                _notification.Show();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new System.Exception("Oops! Something went wrong while adding a new task!", ex);
            }
        }
    }
}
