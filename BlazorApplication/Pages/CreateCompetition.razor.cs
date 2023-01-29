using BlazorApplication.Features;
using BlazorApplication.HttpRepository;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using BlazorApplication.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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
            try
            {
                competitionStatusesList = (List<CompetitionStatus>)await CompetitionRepo.GetAllCompetitionStatuses();
            }
            catch (Exception ex)
            {
                throw new System.Exception("Oops! Something went wrong while getting a list of competition statuses!", ex);
            }
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
            catch(Exception ex)
            {
                throw new System.Exception("Oops! Something went wrong while getting a list of users!", ex);
            }
        }

        private async void Create()
		{
            try 
            {
                await CompetitionRepo.CreateCompetition(_competition);
                _notification.Show();
            }
            catch (Exception ex)
            {
                throw new System.Exception("Oops! Something went wrong while creating a new competition!", ex);
            }
		}

	}
}
