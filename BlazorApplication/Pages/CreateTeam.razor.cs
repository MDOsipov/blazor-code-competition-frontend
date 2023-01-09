using BlazorApplication.HttpRepository;
using BlazorApplication.Models;
using BlazorApplication.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazorApplication.Pages
{
    public partial class CreateTeam
    {
        private Team _team = new Team();

        public List<Competition> competitionList { get; set; } = new List<Competition>();

        private SuccessNotification _notification;

        [Inject]
        public ICompetitionHttpRepository CompetitionRepo { get; set; }

        [Inject]
        public ITeamHttpRepository TeamRepo { get; set; }

		protected async override System.Threading.Tasks.Task OnInitializedAsync()
		{
            competitionList = (List<Competition>)await CompetitionRepo.GetCompetitions();
		}

		private async void Create()
        {
            _team.CreateDate = DateTime.Now;
            _team.UpdateDate = DateTime.Now;
            _team.CreateUserId = 1;
            _team.UpdateUserId = 1;
            _team.StatusId = (int)Enums.Status.Active;
            

            await TeamRepo.CreateTeam(_team);
            _notification.Show();
        }
    }
}
