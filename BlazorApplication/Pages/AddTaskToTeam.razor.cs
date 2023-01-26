using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using BlazorApplication.Shared;
using Microsoft.AspNetCore.Components;
using Task = BlazorApplication.Models.Task;

namespace BlazorApplication.Pages
{
	public partial class AddTaskToTeam
	{
		[Parameter]
		public string teamIdStr { get; set; } = "";

		[Parameter]
		public string participantIdStr { get; set; } = "";
		public int participantId { get; set; } = 0;
		public int competitionId { get; set; } = 0;
		public string navUrlToSend { get; set; } = "";
		public int teamId { get; set; } = 0;
		private SuccessNotification? _notification;
		public int newTaskId { get; set; } = 0;

		[Inject]
		public ITaskToTeamHttpRepository TaskToTeamRepo { get; set; }

		[Inject]
		public ITeamHttpRepository TeamRepo { get; set; }
		

		[Inject]
		public ITaskHttpRepository TaskRepo { get; set; }

		private TaskParameters _taskParameters = new TaskParameters()
		{
			switchOff = true
		};
		public List<Task> TaskList { get; set; } = new List<Task>();
		public List<TaskWithTimesDto> CurrentTasksToTeamInProcess { get; set; } = new List<TaskWithTimesDto>();
		public List<TaskWithTimesDto> CurrentTasksToTeamSubmitted { get; set; } = new List<TaskWithTimesDto>();

		protected async override System.Threading.Tasks.Task OnInitializedAsync()
		{
			if (teamIdStr != "")
			{
				teamId = Int32.Parse(teamIdStr);
			}
            if (participantIdStr != "")
            {
                participantId = Int32.Parse(participantIdStr);
            }
            navUrlToSend = "/myTasks/";
			await GetTasks();
		}

		protected async System.Threading.Tasks.Task GetTasks()
		{
			var pagingResponse = await TaskRepo.GetTasks(_taskParameters);
			TaskList = pagingResponse.Items;
			
			var pagingResponseForCurrentTasksInProcess = await TaskRepo.GetTasksByTeamId(_taskParameters, teamIdStr);
			CurrentTasksToTeamInProcess = pagingResponseForCurrentTasksInProcess.Items;

			var pagingResponseForCurrentTasksSubmitted = await TaskRepo.GetSubmittedTasksByTeamId(_taskParameters, teamIdStr);
			CurrentTasksToTeamSubmitted = pagingResponseForCurrentTasksSubmitted.Items;

			TaskList = TaskList.Where(tl => CurrentTasksToTeamInProcess.Select(ctt => ctt.Id).ToList().IndexOf(tl.Id) == -1).ToList();
			TaskList = TaskList.Where(tl => CurrentTasksToTeamSubmitted.Select(ctt => ctt.Id).ToList().IndexOf(tl.Id) == -1).ToList();

			newTaskId = TaskList.FirstOrDefault().Id;
		}

		

		private async System.Threading.Tasks.Task Create()
		{
			TaskToTeam taskToTeam = new TaskToTeam()
			{
				TeamId = teamId,
				TaskId = newTaskId,
				TaskStatusId = (int)Enums.TaskStatus.InProgress,
				ParticipantIdForTask = participantId,
				ReachedScore = 0,
				StartTime = DateTime.Now,
				EndTime = DateTime.MaxValue
			};
			await TaskToTeamRepo.AddTaskToTeam(taskToTeam);

			_notification.Show();
		}
	}
}
