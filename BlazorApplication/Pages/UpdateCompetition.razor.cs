using BlazorApplication.Features;
using BlazorApplication.HttpRepository;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using BlazorApplication.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Diagnostics;
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
        [Inject]
        public ILogger<UpdateCompetition> Logger { get; set; }

        [Parameter]
        public string Id { get; set; } = "";

        [Parameter]
        public string boolString { get; set; } = "0";

        protected async override Task OnInitializedAsync()
        {
            await GetCompetition();
            await GetStatuses();
            await GetUsers();
        }

        private async Task GetUsers()
        {
            Logger.LogInformation("Get users method is called");
            var userParameters = new UserParameters()
            {
                switchOff = true
            };

            try
            {
                var pagingResponse = await UserRepo.GetUsersExtended(userParameters);
                _users = pagingResponse.Items;
                Logger.LogInformation($"Success. Users: {JsonSerializer.Serialize(_users)}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while getting a list of users!", ex);
            }
        }

        private async Task GetStatuses()
        {
            Logger.LogInformation("Get statuses method is called");
            try
            {
                competitionStatusesList = await CompetitionRepo.GetAllCompetitionStatuses();
                Logger.LogInformation($"Success. Competition statuses: {JsonSerializer.Serialize(competitionStatusesList)}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while getting a list of competition statuses!", ex);
            }
        }

        private async Task GetCompetition()
        {
            Logger.LogInformation("Get competition method is called");
            try
            {
                _competition = await CompetitionRepo.GetCompetitionById(Id);
                Logger.LogInformation($"Success. Competition: {JsonSerializer.Serialize(_competition)}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while getting a list of competitions!", ex);
            }
        }

        private async Task Update()
        {
            Logger.LogInformation("Update method is called");
            try
            {
                await CompetitionRepo.UpdateCompetition(_competition);
                Logger.LogInformation($"Success. The competition is updated");
                StateHasChanged();
                _notification.Show();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while updating the competition!", ex);
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
