using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using BlazorApplication.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using System.Text.Json;

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
        public List<Participant> ParticipantList { get; set; } = new List<Participant>();
		public List<Participant> CurrentParticipantList { get; set; } = new List<Participant>();

		[Inject]
		public IParticipantHttpRepository ParticipantRepo { get; set; }
		[Inject]
		public ILogger<AddParticipantToTeam> Logger { get; set; }

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
			await GetCurrentParticipants();
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
            Logger.LogInformation("Get participants method is called");
            try
			{
                ParticipantList = await ParticipantRepo.GetParticipantsLimited();
				ParticipantList = ParticipantList.Where(pl => CurrentParticipantList.Select(cpl => cpl.id).ToList().IndexOf(pl.id) == -1).ToList();
				newParticipantId = ParticipantList.FirstOrDefault().id;

                Logger.LogInformation($"Success. Participant list: {JsonSerializer.Serialize(ParticipantList)}");
            }
			catch(Exception ex)
			{
                Logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while getting a list of participants!", ex);
            }
        }

		protected async System.Threading.Tasks.Task GetCurrentParticipants()
		{
			Logger.LogInformation("Get current participants method is called");
			try
			{
				ParticipantParameters participantParameters = new ParticipantParameters()
				{
					switchOff = true
				};

				var pagingResponse = await ParticipantRepo.GetParticipantsByTeamId(participantParameters, teamIdStr);
				CurrentParticipantList = pagingResponse.Items;

				Logger.LogInformation($"Success. Current participant list: {JsonSerializer.Serialize(CurrentParticipantList)}");
			}
			catch (Exception ex)
			{
				Logger.LogError($"Error: {ex}");
				throw new Exception("Oops! Something went wrong while getting a list of current participants!", ex);
			}
		}
		private async void Create()
		{
            Logger.LogInformation("Create method is called");
            try
            {
                await ParticipantRepo.AddTeamToParticipant(teamIdStr, newParticipantId.ToString());
                Logger.LogInformation($"Success. New team for the participant is successfully added");
                _notification.Show();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while adding a new participant!", ex);
            }
		}
	}
}

