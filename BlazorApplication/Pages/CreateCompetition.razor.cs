using BlazorApplication.Features;
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
    public partial class CreateCompetition
    {

        [Inject]
        public IUserHttpRepository UserRepo { get; set; }

        private IEnumerable<UserDto> _users = new List<UserDto>();
        private Competition _competition = new Competition();
        public List<CompetitionStatus> competitionStatusesList { get; set; } = new List<CompetitionStatus>();

        private SuccessNotification _notification;
        private ErrorBoundary? errorBoundary;

        [Inject]
        public ICompetitionHttpRepository CompetitionRepo { get; set; }
        [Inject]
        public ILogger<CreateCompetition> Logger { get; set; }

		protected async override System.Threading.Tasks.Task OnInitializedAsync()
		{
			await GetStatuses();
            await GetUsers();
            _competition.CompetitionAdministratorId = _users.FirstOrDefault().Id;
            _competition.CompetitionStatusId = competitionStatusesList.FirstOrDefault().Id;
        }
        protected override void OnParametersSet()
        {
            errorBoundary?.Recover();
        }
        private void ResetError()
        {
            errorBoundary?.Recover();
        }
        private async Task GetStatuses()
		{
            Logger.LogInformation("Get statuses method is called");
            try
            {
                competitionStatusesList = (List<CompetitionStatus>)await CompetitionRepo.GetAllCompetitionStatuses();
                Logger.LogInformation($"Success. Competition statuses: {JsonSerializer.Serialize(competitionStatusesList)}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new System.Exception("Oops! Something went wrong while getting a list of competition statuses!", ex);
            }
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
                throw new System.Exception("Oops! Something went wrong while getting a list of users!", ex);
            }
        }

        private async void Create()
		{
            Logger.LogInformation("Create method is called");
            try
            {
                await CompetitionRepo.CreateCompetition(_competition);
                _notification.Show();
                Logger.LogInformation($"Success. A new competition is created");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new System.Exception("Oops! Something went wrong while creating a new competition!", ex);
            }
		}

	}
}
