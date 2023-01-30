using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Text.Json;

namespace BlazorApplication.Pages
{
	public partial class TeamParticipantsManagement
	{
		[Parameter]
		public string id { get; set; } = "";

		[Inject]
		public IUserHttpRepository userRepo { get; set; }

        [Inject]
        public IParticipantHttpRepository participantRepo { get; set; }

        [Inject]
        public ILogger<TeamParticipantsManagement> Logger { get; set; }
        public List<Models.Participant> ParticipantList { get; set; } = new List<Models.Participant>();
		public MetaData MetaData { get; set; } = new MetaData();
		private ParticipantParameters _participantParameters = new ParticipantParameters();
        private ErrorBoundary? errorBoundary;

        [Inject]
		public ITaskHttpRepository TaskRepo { get; set; }

		protected async override System.Threading.Tasks.Task OnInitializedAsync()
		{
			await GetParticipants();
		}

		private async System.Threading.Tasks.Task SelectedPage(int page)
		{
            _participantParameters.PageNumber = page;
			await GetParticipants();
		}
		protected async System.Threading.Tasks.Task GetParticipants()
		{
            Logger.LogInformation("Get participants method is called");
            try
            {
                var pagingResponse = await participantRepo.GetParticipantsByTeamId(_participantParameters, id);
                ParticipantList = pagingResponse.Items;
                MetaData = pagingResponse.MetaData;
                Logger.LogInformation($"Success. Participants: {JsonSerializer.Serialize(ParticipantList)}");
            }
            catch (Exception ex)
			{
                Logger.LogError($"Error: {ex}");
                throw new System.Exception("Oops! Something went wrong while getting a list of participants!", ex);
            }
        }
		private async System.Threading.Tasks.Task DeleteParticipant(int participantId)
		{
            Logger.LogInformation("Delete participant method is called");
            try
            {
                await participantRepo.RemoveTeamFromParticipant(participantId.ToString());
                _participantParameters.PageNumber = 1;
                Logger.LogInformation($"Success. The participant is deleted");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new System.Exception("Oops! Something went wrong while removing a participant from a team!", ex);
            }
            await GetParticipants(); 
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
