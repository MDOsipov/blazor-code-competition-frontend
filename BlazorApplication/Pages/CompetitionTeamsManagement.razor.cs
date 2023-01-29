using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using Microsoft.AspNetCore.Components;

namespace BlazorApplication.Pages
{
    public partial class CompetitionTeamsManagement
    {
        [Parameter]
        public string id { get; set; } = "";

        [Parameter]
        public bool successResponse { get; set; }

        [Inject]
        public ITaskToCompetitionRepository taskToCompetitionRepo { get; set; }

        public List<Team> TeamList { get; set; } = new List<Team>();
        public MetaData MetaData { get; set; } = new MetaData();
        private TeamParameters _teamParameters = new TeamParameters();

        [Inject]
        public ITeamHttpRepository TeamRepo { get; set; }

        protected async override System.Threading.Tasks.Task OnInitializedAsync()
        {
            await GetTeams();
        }

        private async System.Threading.Tasks.Task SelectedPage(int page)
        {
            _teamParameters.PageNumber = page;
            await GetTeams();
        }

        protected async System.Threading.Tasks.Task GetTeams()
        {
            var pagingResponse = await TeamRepo.GetTeams(_teamParameters);
            TeamList = pagingResponse.Items.Where(t => t.CompetitionId == Int32.Parse(id)).ToList();
            MetaData = pagingResponse.MetaData;
            successResponse = pagingResponse.SuccessRequest;
        }

        
        private async System.Threading.Tasks.Task DeleteTeam(int teamId)
        {
            Team newTeam = await TeamRepo.GetTeamById(teamId.ToString());
            newTeam.CompetitionId = null;

            await TeamRepo.UpdateTeam(newTeam);

            _teamParameters.PageNumber = 1;
            await GetTeams();
        }

    }
}
