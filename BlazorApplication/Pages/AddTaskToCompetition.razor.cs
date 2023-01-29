﻿using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using BlazorApplication.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Diagnostics;
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
        public ITaskToCompetitionRepository TaskToCompetitionRepo { get; set; }

        private TaskParameters _taskParameters = new TaskParameters()
		{
			switchOff = true
		};
		public List<Task> TaskList { get; set; } = new List<Task>();

		protected async override System.Threading.Tasks.Task OnInitializedAsync()
		{
			if (competitionIdStr != "")
			{
				competitionId = Int32.Parse(competitionIdStr);
				navUrlToSend = "/competitionTasksManagement/" + competitionIdStr.ToString();
            }
			await GetTasks();
		}

		protected async System.Threading.Tasks.Task GetTasks()
		{
			try
			{
                var pagingResponse = await TaskRepo.GetTasks(_taskParameters);
                TaskList = pagingResponse.Items;
                newTaskId = TaskList.FirstOrDefault().Id;
            }
			catch (Exception ex)
			{
                throw new System.Exception("Oops! Something went wrong while getting a list of tasks!", ex);
            }
        }

        private async void Create()
        {
			TaskToCompetition taskToCompetition = new TaskToCompetition()
			{
				CompetitionId = competitionId,
				TaskId = newTaskId
			};

            try 
            {
                await TaskToCompetitionRepo.AddTaskToCompetition(taskToCompetition);
                _notification.Show();
            }
            catch (Exception ex)
            {
                throw new System.Exception("Oops! Something went wrong while adding a new task!", ex);
            }
        }
    }
}