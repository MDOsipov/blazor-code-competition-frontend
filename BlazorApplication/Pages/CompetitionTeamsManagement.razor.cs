using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

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
        private ErrorBoundary? errorBoundary;

        [Inject]
        public ITeamHttpRepository TeamRepo { get; set; }
        [Inject]
        public ILogger<CompetitionTeamsManagement> Logger { get; set; } 

        protected async override System.Threading.Tasks.Task OnInitializedAsync()
        {
            await GetTeams();
        }
        protected override void OnParametersSet()
        {
            errorBoundary?.Recover();
        }
        private void ResetError()
        {
            errorBoundary?.Recover();
        }
        private async System.Threading.Tasks.Task SelectedPage(int page)
        {
            _teamParameters.PageNumber = page;
            await GetTeams();
        }

        protected async System.Threading.Tasks.Task GetTeams()
        {
            Logger.LogInformation("Get teams method is called");
            try
            {
                var pagingResponse = await TeamRepo.GetTeams(_teamParameters);
                TeamList = pagingResponse.Items.Where(t => t.CompetitionId == Int32.Parse(id)).ToList();
                MetaData = pagingResponse.MetaData;
                successResponse = pagingResponse.SuccessRequest;
                Logger.LogInformation($"Success. Teams: {JsonSerializer.Serialize(TeamList)}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new System.Exception("Oops! Something went wrong while getting a list of teams for this competition!", ex);
            }
        }

        
        private async System.Threading.Tasks.Task DeleteTeam(int teamId)
        {
            Logger.LogInformation("Delete team method is called");
            Team newTeam;
            try
            {
                newTeam = await TeamRepo.GetTeamById(teamId.ToString());
                newTeam.CompetitionId = null;
            }
            catch(Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new System.Exception("Oops! Something went wrong while getting a team to delete!", ex);
            }

            try
            {
                await TeamRepo.UpdateTeam(newTeam);
                _teamParameters.PageNumber = 1;
                Logger.LogInformation($"Success. The team is removed from the competition");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new System.Exception("Oops! Something went wrong while deleting a team for the competition!", ex);
            }
            
            await GetTeams();
        }

    }
}
