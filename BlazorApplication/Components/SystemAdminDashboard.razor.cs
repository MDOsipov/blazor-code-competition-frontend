using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using Microsoft.AspNetCore.Components;
using System.Net.NetworkInformation;
using static System.Net.WebRequestMethods;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;



namespace BlazorApplication.Components
{
    public partial class SystemAdminDashboard
    {
		private Models.BackEndConnections _backEndConnections;

        public List<Competition> CompetitionList { get; set; } = new List<Competition>();
		public IEnumerable<UserDto> UserList { get; set; } = new List<UserDto>();
		private UserParameters userParameters = new UserParameters();

        [Inject]
		public ICompetitionHttpRepository CompetitionRepo { get; set; }

        [Inject]
        public IUserHttpRepository UserRepo { get; set; }

        [Inject]
		public HttpClient client { get; set; }

		[Inject]
		public IConfiguration configuration { get; set; }

		protected async override System.Threading.Tasks.Task OnInitializedAsync()
		{
			_backEndConnections = configuration.GetSection("ConnectionStrings").Get<Models.BackEndConnections>();
			await GetCompetitions();
			await GetUsers();
        }

		protected async System.Threading.Tasks.Task GetCompetitions()
		{
			CompetitionParameters competitionParameters = new CompetitionParameters
			{
				switchOff = true
			};

			var pagingResponse = await CompetitionRepo.GetCompetitions(competitionParameters);
			CompetitionList = pagingResponse.Items;
		}

        protected async System.Threading.Tasks.Task GetUsers()
        {
            var PagingResponse = await UserRepo.GetUsersExtended(userParameters);
            UserList = PagingResponse.Items;
        }
    }
}
