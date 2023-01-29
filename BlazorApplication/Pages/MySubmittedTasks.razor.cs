﻿using BlazorApplication.Features;
using BlazorApplication.HttpRepository;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using Microsoft.AspNetCore.Components;
using System.Text.Json;
using Task = System.Threading.Tasks.Task;

namespace BlazorApplication.Pages
{
    public partial class MySubmittedTasks
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
		public ITaskHttpRepository TaskRepo { get; set; }

        [Parameter]
        public bool successResponse { get; set; }
		public List<TaskWithTimesDto> TaskList { get; set; } = new List<TaskWithTimesDto>();
        public MetaData MetaData { get; set; } = new MetaData();
        private TaskParameters _taskParameters = new TaskParameters();

        private string LogedUserId = "";
        private string LogedUserEmail = "";
        private int UserTeamId = 0;
        private int ParticipantId = 0;
        private string UserTeamName = "";

        protected async override Task OnInitializedAsync()
        {
            await GetUserId();
            await GetUserEmail();
            await GetUserTeam();
            await GetTasksByTeamId();
        }
        private async Task GetUserId()
        {
            var claims = await AuthTest.GetClaims();
            LogedUserId = claims.Where(c => c.Type == "sub").FirstOrDefault().Value.ToString();
            Console.WriteLine("Our user id: " + LogedUserId);
        }

        private async Task GetUserEmail()
        {
            UserParameters userParameters = new UserParameters()
            {
                switchOff = true
            };

            var pagingResponse = await UserRepo.GetUsersExtended(userParameters);
            var users = pagingResponse.Items;
            LogedUserEmail = users.Where(u => u.Id == LogedUserId).FirstOrDefault().Email;
            Console.WriteLine("User's email: ");
            Console.WriteLine(LogedUserEmail);
        }

        private async Task GetUserTeam()
        {
            Console.WriteLine("Started get user team");
            ParticipantParameters participantParameters = new ParticipantParameters()
            {
                switchOff = true
            };

            var pagingResponse = await ParticipantRepo.GetParticipantsByEmail(participantParameters, LogedUserEmail);
            var participant = pagingResponse.Items.FirstOrDefault();
            Console.WriteLine("Participant: ");
            Console.WriteLine(JsonSerializer.Serialize(participant));

            if (participant?.teamId is not null)
            {
                UserTeamId = (int)participant.teamId;
                ParticipantId = participant.id;
                Console.WriteLine("User's team id: ");
                Console.WriteLine(UserTeamId);
            }

            if (UserTeamId > 0)
            {
                var team = await TeamRepo.GetTeamById(UserTeamId.ToString());
                UserTeamName = team.TeamName;
                Console.WriteLine("User's team name: ");
                Console.WriteLine(UserTeamName);
            }
            else
            {
                UserTeamName = "No team";
                Console.WriteLine("User's team name: ");
                Console.WriteLine(UserTeamName);
            }

        }
        private async Task SelectedPage(int page)
        {
            _taskParameters.PageNumber = page;
            await GetTasksByTeamId();
        }

        protected async Task GetTasksByTeamId()
        {
            var pagingResponse = await TaskRepo.GetSubmittedTasksByTeamId(_taskParameters, UserTeamId.ToString());
            TaskList = pagingResponse.Items;
            MetaData = pagingResponse.MetaData;
            successResponse = pagingResponse.SuccessRequest;
        }

    }
}

