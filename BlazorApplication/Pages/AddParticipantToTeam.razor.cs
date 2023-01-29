using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using BlazorApplication.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorApplication.Pages
{
	public partial class AddParticipantToTeam
	{
		[Parameter]
		public string teamIdStr { get; set; } = "";
		private ErrorBoundary? errorBoundary;

		public string navUrlToSend { get; set; } = "";
		public int teamId { get; set; } = 0;
		private SuccessNotification? _notification;
		public int newParticipantId { get; set; } = 0;

		[Inject]
		public IParticipantHttpRepository ParticipantRepo { get; set; }


		private ParticipantParameters _participantParameters = new ParticipantParameters()
		{
			switchOff = true
		};
		public List<Participant> ParticipantList { get; set; } = new List<Participant>();

		protected async override System.Threading.Tasks.Task OnInitializedAsync()
		{
			if (teamIdStr != "")
			{
				teamId = Int32.Parse(teamIdStr);
				navUrlToSend = "/teamParticipantsManagement/" + teamIdStr;
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
		protected async System.Threading.Tasks.Task GetParticipants()
		{
			try
			{
                ParticipantList = (List<Participant>)await ParticipantRepo.GetParticipantsLimited();
                newParticipantId = ParticipantList.FirstOrDefault().id;
            }
			catch(Exception ex)
			{
                throw new System.Exception("Oops! Something went wrong while getting a list of participants!", ex);
            }
        }

		private async void Create()
		{
			try
			{
                await ParticipantRepo.AddTeamToParticipant(teamIdStr, newParticipantId.ToString());
                _notification.Show();
            }
            catch (Exception ex)
            {
                throw new System.Exception("Oops! Something went wrong while adding a new participant!", ex);
            }
		}
	}
}

