using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using Microsoft.AspNetCore.Components;

namespace BlazorApplication.Pages
{
    public partial class Participants
	{
		public List<Participant> ParticipantsList { get; set; } = new List<Participant>();
		public MetaData MetaData { get; set; } = new MetaData();
		private ParticipantParameters _participantParameters = new ParticipantParameters();

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
			var pagingResponse = await ParticipantRepo.GetParticipants(_participantParameters);
			ParticipantsList = pagingResponse.Items;
			MetaData = pagingResponse.MetaData;
		}

		private async System.Threading.Tasks.Task DeleteParticipant(int id)
		{
			await ParticipantRepo.DeleteParticipant(id);
			_participantParameters.PageNumber = 1;
			await GetParticipants();
		}
	}
}
