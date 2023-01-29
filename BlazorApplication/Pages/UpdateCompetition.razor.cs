﻿using BlazorApplication.Features;
using BlazorApplication.HttpRepository;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using BlazorApplication.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Text.Json;
using Task = System.Threading.Tasks.Task;

namespace BlazorApplication.Pages
{
    public partial class UpdateCompetition
    {
        private IEnumerable<UserDto> _users = new List<UserDto>();
		private Competition _competition = new Competition();
        public List<CompetitionStatus> competitionStatusesList { get; set; } = new List<CompetitionStatus>();
        private SuccessNotification _notification;
        private ErrorBoundary? errorBoundary;

        [Inject]
		public IUserHttpRepository UserRepo { get; set; }
		[Inject]
        public ICompetitionHttpRepository CompetitionRepo { get; set; }

        [Parameter]
        public string Id { get; set; } = "";

        [Parameter]
        public string boolString { get; set; } = "0";

        protected async override System.Threading.Tasks.Task OnInitializedAsync()
        {
            await GetCompetition();
            await GetStatuses();
            await GetUsers();
        }

        private async Task GetUsers()
        {
            var userParameters = new UserParameters()
            {
                switchOff = true
            };

            try
            {
                var pagingResponse = await UserRepo.GetUsersExtended(userParameters);
                _users = pagingResponse.Items;
            }
            catch (Exception ex)
            {
                throw new System.Exception("Oops! Something went wrong while getting a list of users!", ex);
            }
        }

        private async Task GetStatuses()
        {
            try
            {
                competitionStatusesList = (List<CompetitionStatus>)await CompetitionRepo.GetAllCompetitionStatuses();
            }
            catch (Exception ex)
            {
                throw new System.Exception("Oops! Something went wrong while getting a list of competition statuses!", ex);
            }
        }

        private async Task GetCompetition()
        {
            try
            {
                _competition = await CompetitionRepo.GetCompetitionById(Id);
                Console.WriteLine("Got competition object: ");
                Console.WriteLine(JsonSerializer.Serialize(_competition));
            }
            catch(Exception ex)
            {
                throw new System.Exception("Oops! Something went wrong while getting a list of competitions!", ex);
            }
        }

        private async Task Update()
        {
            try
            {
                await CompetitionRepo.UpdateCompetition(_competition);
                StateHasChanged();
                _notification.Show();
            }
            catch (Exception ex)
            {
                throw new System.Exception("Oops! Something went wrong while updating the competition!", ex);
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
