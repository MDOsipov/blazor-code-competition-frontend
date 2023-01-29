﻿using BlazorApplication.Features;
using BlazorApplication.HttpRepository;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Text.Json;
using Task = System.Threading.Tasks.Task;

namespace BlazorApplication.Pages
{
	public partial class MyTasks
	{
        [Inject]
        public AuthTest AuthTest { get; set; }
        [Inject]
        public IUserHttpRepository UserRepo { get; set; }
        [Inject]
        public IParticipantHttpRepository ParticipantRepo { get; set; }
		[Inject]
		public ITaskToTeamHttpRepository TaskToTeamRepo { get; set; }
		[Inject]
        public ITeamHttpRepository TeamRepo { get; set; }
        [Inject]
        public ICompetitionHttpRepository CompetitionRepo { get; set; }
        public List<Models.TaskWithTimesDto> TaskList { get; set; } = new List<Models.TaskWithTimesDto>();
        public MetaData MetaData { get; set; } = new MetaData();
        private TaskParameters _taskParameters = new TaskParameters();
        private ErrorBoundary? errorBoundary;

        private string LogedUserId = "";
        private string LogedUserEmail = "";
        private int UserTeamId = 0;
        private int ParticipantId = 0;
        private string UserTeamName = "";
        private int CompetitionId = 0;
        private int maxNumTasks = 0;


        [Inject]
        public ITaskHttpRepository TaskRepo { get; set; }

        protected async override System.Threading.Tasks.Task OnInitializedAsync()
        {
            await GetUserId();
            await GetUserEmail();
            await GetUserTeam();
            await GetCompetitionId();
            await GetMaxNumTasks();
            await GetTasksByTeamId();
        }
        private async Task GetUserId()
        {
            try
            {
                var claims = await AuthTest.GetClaims();
                LogedUserId = claims.Where(c => c.Type == "sub").FirstOrDefault().Value.ToString();
                Console.WriteLine("Our user id: " + LogedUserId);
            }
            catch (Exception ex)
            {
                throw new System.Exception("Oops! Something went wrong while getting user info!", ex);
            }
        }

        private async Task GetUserEmail()
        {
            UserParameters userParameters = new UserParameters()
            {
                switchOff = true
            };

            try
            {
                var pagingResponse = await UserRepo.GetUsersExtended(userParameters);
                var users = pagingResponse.Items;
                LogedUserEmail = users.Where(u => u.Id == LogedUserId).FirstOrDefault().Email;
                Console.WriteLine("User's email: ");
                Console.WriteLine(LogedUserEmail);
            }
            catch (Exception ex)
            {
                throw new System.Exception("Oops! Something went wrong while getting user info!", ex);
            }
        }

        private async Task GetUserTeam()
        {
            Console.WriteLine("Started get user team");
            ParticipantParameters participantParameters = new ParticipantParameters()
            {
                switchOff = true
            };

            Participant participant;
            try
            {
                var pagingResponse = await ParticipantRepo.GetParticipantsByEmail(participantParameters, LogedUserEmail);
                participant = pagingResponse.Items.FirstOrDefault();
                Console.WriteLine("Participant: ");
                Console.WriteLine(JsonSerializer.Serialize(participant));
            }
            catch (Exception ex)
            {
                throw new System.Exception("Oops! Something went wrong while getting user info!", ex);
            }
            

            if (participant?.teamId is not null)
            {
                UserTeamId = (int)participant.teamId;
                ParticipantId = participant.id;
                Console.WriteLine("User's team id: ");
                Console.WriteLine(UserTeamId);
            }

            if (UserTeamId > 0)
            {
                try
                {
                    var team = await TeamRepo.GetTeamById(UserTeamId.ToString());
                    UserTeamName = team.TeamName;
                    Console.WriteLine("User's team name: ");
                    Console.WriteLine(UserTeamName);
                }
                catch (Exception ex)
                {
                    throw new System.Exception("Oops! Something went wrong while getting user team!", ex);
                }
            }
            else
            {
                UserTeamName = "No team";
                Console.WriteLine("User's team name: ");
                Console.WriteLine(UserTeamName);
            }

        }
        private async System.Threading.Tasks.Task SelectedPage(int page)
        {
            _taskParameters.PageNumber = page;
            await GetTasksByTeamId();
        }

        protected async System.Threading.Tasks.Task GetTasksByTeamId()
        {
            try
            {
                var pagingResponse = await TaskRepo.GetTasksByTeamId(_taskParameters, UserTeamId.ToString());
                TaskList = pagingResponse.Items;
                MetaData = pagingResponse.MetaData;
            }
            catch (Exception ex)
            {
                throw new System.Exception("Oops! Something went wrong while getting a list of tasks!", ex);
            }
        }

		private async System.Threading.Tasks.Task RemoveTaskFromTeam(int taskId)
		{
            try
            {
                await TaskToTeamRepo.DeleteTeamTaskByTaskIdAndTeamId(taskId.ToString(), UserTeamId.ToString());
                _taskParameters.PageNumber = 1;
            }
            catch (Exception ex)
            {
                throw new System.Exception("Oops! Something went wrong while removing a task!", ex);
            }
            
			await GetTasksByTeamId();
		}

        private async System.Threading.Tasks.Task GetCompetitionId()
        {
            Team team;
            try
            {
                team = await TeamRepo.GetTeamById(UserTeamId.ToString());
            }
            catch (Exception ex)
            {
                throw new System.Exception("Oops! Something went wrong while getting a user team!", ex);
            }

            if (team.CompetitionId is not null)
            {
                CompetitionId = (int)team.CompetitionId;
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

        private async System.Threading.Tasks.Task GetMaxNumTasks()
        {
            if (CompetitionId > 0)
            {
                Competition competition;
                try
                {
                    competition = await CompetitionRepo.GetCompetitionById(CompetitionId.ToString());
                }
                catch (Exception ex)
                {
                    throw new System.Exception("Oops! Something went wrong while getting current competition!", ex);
                }

                if (competition is not null)
                {
                    maxNumTasks = competition.maxTaskPerGroups;
                    Console.WriteLine("Max possible task number = " + maxNumTasks.ToString());
                }
            }
        }

    }
}