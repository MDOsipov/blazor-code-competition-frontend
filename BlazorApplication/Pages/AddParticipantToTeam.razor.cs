using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using BlazorApplication.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazorApplication.Pages
{
	public partial class AddParticipantToTeam
	{
		[Parameter]
		public string teamIdStr { get; set; } = "";

		public string navUrlToSend { get; set; } = "";
		public int teamId { get; set; } = 0;
		private SuccessNotification? _notification;
		public int newParticipantId { get; set; } = 0;
        public List<Participant> ParticipantList { get; set; } = new List<Participant>();

        [Inject]
		public IParticipantHttpRepository ParticipantRepo { get; set; }


		private ParticipantParameters _participantParameters = new ParticipantParameters()
		{
			switchOff = true
		};
		

		protected async override System.Threading.Tasks.Task OnInitializedAsync()
		{
			if (teamIdStr != "")
			{
				teamId = Int32.Parse(teamIdStr);
				navUrlToSend = "/teamParticipantsManagement/" + teamIdStr;
			}
			await GetParticipants();
		}

		protected async System.Threading.Tasks.Task GetParticipants()
		{
            /*
			var pagingResponse = await ParticipantRepo.GetParticipants(_participantParameters);
			ParticipantList = pagingResponse.Items;
			newParticipantId = ParticipantList.FirstOrDefault().id; */

            var responseWithSuccess = await ParticipantRepo.GetParticipantsLimited();
			ParticipantList = responseWithSuccess.Items;
            newParticipantId = ParticipantList.FirstOrDefault().id;
        }

		private async void Create()
		{
			await ParticipantRepo.AddTeamToParticipant(teamIdStr, newParticipantId.ToString());
			_notification.Show();
		}
	}
}

