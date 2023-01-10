using BlazorApplication.HttpRepository;
using BlazorApplication.Models;
using Microsoft.AspNetCore.Components;

namespace BlazorApplication.Pages
{
	public partial class Participants
	{
		public List<Participant> ParticipantsList { get; set; } = new List<Participant>();
		// public MetaData MetaData { get; set; } = new MetaData();
		// private TeamParameters _teamParameters = new TeamParameters();

		[Inject]
		public IParticipantHttpRepository ParticipantRepo { get; set; }

		protected async override System.Threading.Tasks.Task OnInitializedAsync()
		{
			await GetParticipants();
		}

		/*
        private async System.Threading.Tasks.Task SelectedPage(int page)
        {
            _teamParameters.PageNumber = page;
            await GetTeams();
        }
        */

		protected async System.Threading.Tasks.Task GetParticipants()
		{
			ParticipantsList = (List<Participant>)await ParticipantRepo.GetParticipants();
		}
	}
}
