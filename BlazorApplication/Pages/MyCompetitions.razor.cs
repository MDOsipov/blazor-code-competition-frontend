using BlazorApplication.Features;
using BlazorApplication.HttpRepository;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using System.Text.Json;
using static Microsoft.AspNetCore.Components.Authorization.AuthenticationState;
using Task = System.Threading.Tasks.Task;

namespace BlazorApplication.Pages
{
	public partial class MyCompetitions
	{
		[Inject]
		public ICompetitionHttpRepository CompetitionRepo { get; set; }

		[Inject]
		public ILogger<MyCompetitions> Logger { get; set; }

		[Inject]
		public AuthTest authTest { get; set; }
		public List<Competition> CompetitionList { get; set; } = new List<Competition>();
		public MetaData MetaData { get; set; } = new MetaData();
		private CompetitionParameters _competitionParameters = new CompetitionParameters();
        private ErrorBoundary? errorBoundary;

		private string LogedUserId = "";

		[Parameter]
		public bool successResponse { get; set; }

		protected async override Task OnInitializedAsync()
		{            
			await LogUsername();
			await GetUserId();
			await GetCompetitions();
		}

		protected async Task GetCompetitions()
		{
            Logger.LogInformation("Get competition method is called");
            try
            {
                var pagingResponse = await CompetitionRepo.GetCompetitionsByAdminId(LogedUserId, _competitionParameters);
                CompetitionList = pagingResponse.Items;
                MetaData = pagingResponse.MetaData;
                successResponse = pagingResponse.SuccessRequest;
                Logger.LogInformation($"Success. Competitions: {JsonSerializer.Serialize(CompetitionList)}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while getting a list of competitions!", ex);
            }
		}
		private async Task DeleteCompetition(int id)
		{
            Logger.LogInformation("Delete competition method is called");
            try
            {
                await CompetitionRepo.DeleteCompetition(id);
                _competitionParameters.PageNumber = 1;
                Logger.LogInformation($"Success. The competition is removed from the list of my competitions");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while removing a competition!", ex);
            }
			await GetCompetitions();
		}
		private async Task SelectedPage(int page)
		{
			_competitionParameters.PageNumber = page;
			await GetCompetitions();
		}

		private async Task LogUsername()
		{
			var identity = await authTest.GetIdentity();
		}

		private async Task LogClaims()
		{
			var claims = await authTest.GetClaims();

			foreach(var claim in claims)
			{
				Console.WriteLine(claim.Type + " " + claim.Value);
			}
		}

		private async Task GetUserId()
		{
            Logger.LogInformation("Get user id method is called");
            try
            {
                var claims = await authTest.GetClaims();
                LogedUserId = claims.Where(c => c.Type == "sub").FirstOrDefault().Value.ToString();
                Logger.LogInformation($"Success. User id: {JsonSerializer.Serialize(LogedUserId)}");
            }
            catch (Exception ex)
			{
                Logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while getting user info!", ex);
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
