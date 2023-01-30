using BlazorApplication.Features;
using BlazorApplication.HttpRepository;
using BlazorApplication.Interfaces;
using Microsoft.AspNetCore.Components;
using System.Text.Json;

namespace BlazorApplication.Components
{
    public partial class ParticipantDashboard
    {
        [Inject]
        public AuthTest? AuthTest { get; set; }

        [Inject]
        public IUserHttpRepository UserRepo { get; set; }

		[Inject]
		public IParticipantHttpRepository ParticipantRepo { get; set; }

		[Inject]
		public ITeamHttpRepository TeamRepo { get; set; }

		private string LogedUserId = "";
        private string LogedUserEmail = string.Empty;
        private int UserTeamId = 0;
        private string UserTeamName = string.Empty;


        protected async override System.Threading.Tasks.Task OnInitializedAsync()
        {
            await GetUserId();
			//Console.WriteLine("Step 1");
			await GetUserEmail();
			//Console.WriteLine("Step 2");
			await GetUserTeam();
		}

        private async Task GetUserId()
        {
            var claims = await AuthTest.GetClaims();
            LogedUserId = claims.Where(c => c.Type == "sub").FirstOrDefault().Value.ToString();
            //Console.WriteLine("Our user id: " + LogedUserId);
        }

        private async Task GetUserEmail()
        {
            UserParameters userParameters = new UserParameters()
            {
                switchOff = true
            };

			var pagingResponse = await UserRepo.GetUsersExtended(userParameters);
            var users = pagingResponse.Items;
            LogedUserEmail = users.Where(u => u.Id == LogedUserId).FirstOrDefault().Email;
            //Console.WriteLine("User's email: ");
            //Console.WriteLine(LogedUserEmail); 
		}

        private async Task GetUserTeam()
        {
            //Console.WriteLine("Started get user team");
            ParticipantParameters participantParameters = new ParticipantParameters()
            {
                switchOff = true
            };

            var pagingResponse = await ParticipantRepo.GetParticipantsByEmail(participantParameters, LogedUserEmail);
            var participant = pagingResponse.Items.FirstOrDefault();
			//Console.WriteLine("Participant: ");
			//Console.WriteLine(JsonSerializer.Serialize(participant));

			if (participant?.teamId is not null)
            {
				UserTeamId = (int)participant.teamId;
				//Console.WriteLine("User's team id: ");
				//Console.WriteLine(UserTeamId);
			}

			if (UserTeamId > 0)
            {
				var team = await TeamRepo.GetTeamById(UserTeamId.ToString());
                UserTeamName = team.TeamName;
				//Console.WriteLine("User's team name: ");
				//Console.WriteLine(UserTeamName);
			}
            else
            {
                UserTeamName = "No team";
				//Console.WriteLine("User's team name: ");
				//Console.WriteLine(UserTeamName);
			}

		}

    }
}
