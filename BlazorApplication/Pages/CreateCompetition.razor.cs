using BlazorApplication.Features;
using BlazorApplication.HttpRepository;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using BlazorApplication.Shared;
using Microsoft.AspNetCore.Components;
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

		[Inject]
		public ICompetitionHttpRepository CompetitionRepo { get; set; }


		protected async override System.Threading.Tasks.Task OnInitializedAsync()
		{
			await GetStatuses();
            await GetUsers();
            _competition.CompetitionAdministratorId = _users.FirstOrDefault().Id;
            _competition.CompetitionStatusId = competitionStatusesList.FirstOrDefault().Id;
        }

        private async Task GetStatuses()
		{
            var responseWithStatues = await CompetitionRepo.GetAllCompetitionStatuses();
            competitionStatusesList = responseWithStatues.Items;
        }

        private async Task GetUsers()
        {
            var userParameters = new UserParameters()
            {
                switchOff = true
            };
            var pagingResponse = await UserRepo.GetUsersExtended(userParameters);
            _users = pagingResponse.Items;
        }

        private async void Create()
		{
			await CompetitionRepo.CreateCompetition(_competition);
			_notification.Show();
		}

	}
}
