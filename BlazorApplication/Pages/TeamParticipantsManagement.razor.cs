using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using Microsoft.AspNetCore.Components;
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

        public List<Models.Participant> ParticipantList { get; set; } = new List<Models.Participant>();
		public MetaData MetaData { get; set; } = new MetaData();
		private ParticipantParameters _participantParameters = new ParticipantParameters();

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
			var pagingResponse = await participantRepo.GetParticipantsByTeamId(_participantParameters, id);
			ParticipantList = pagingResponse.Items;
			MetaData = pagingResponse.MetaData;
			//Console.WriteLine("Participant list: " + JsonSerializer.Serialize(ParticipantList));
		}

		private async System.Threading.Tasks.Task DeleteTask(int participantId)
		{
			await participantRepo.RemoveTeamFromParticipant(participantId.ToString());
			_participantParameters.PageNumber = 1;
			await GetParticipants(); 
		}

	}
}
