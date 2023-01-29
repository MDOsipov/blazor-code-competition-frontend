using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorApplication.Pages
{
    public partial class Participants
	{
		public List<Participant> ParticipantsList { get; set; } = new List<Participant>();
		public MetaData MetaData { get; set; } = new MetaData();
		private ParticipantParameters _participantParameters = new ParticipantParameters();
        private ErrorBoundary? errorBoundary;

        [Inject]
		public IParticipantHttpRepository ParticipantRepo { get; set; }

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
			try
			{
                var pagingResponse = await ParticipantRepo.GetParticipants(_participantParameters);
                ParticipantsList = pagingResponse.Items;
                MetaData = pagingResponse.MetaData;
            }
            catch(Exception ex)
            {
                throw new System.Exception("Oops! Something went wrong while getting a list of participants!", ex);
            }

        }

		private async System.Threading.Tasks.Task DeleteParticipant(int id)
		{
            try
            {
                await ParticipantRepo.DeleteParticipant(id);
            }
            catch(Exception ex)
            {
                throw new System.Exception("Oops! Something went wrong while deleting a participant!", ex);
            }

            _participantParameters.PageNumber = 1;
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
