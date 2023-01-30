using BlazorApplication.Features;
using BlazorApplication.HttpRepository;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using Microsoft.AspNetCore.Components;
using System.Text.Json;
using Task = System.Threading.Tasks.Task;

namespace BlazorApplication.Pages
{
	public partial class MyTasks
	{
        [Inject]
        public AuthTest AuthTest { get; set; }
        [Inject]
        public IUserHttpRepository UserRepo { get; set; }
        [Inject]
        public IParticipantHttpRepository ParticipantRepo { get; set; }
		[Inject]
		public ITaskToTeamHttpRepository TaskToTeamRepo { get; set; }
		[Inject]
        public ITeamHttpRepository TeamRepo { get; set; }
        [Inject]
        public ICompetitionHttpRepository CompetitionRepo { get; set; }

		[Inject]
		public ITaskHttpRepository TaskRepo { get; set; }

        [Parameter]
        public bool successResponse { get; set; }

        [Parameter]
        public bool successResponseParticipant { get; set; } = false;
		public List<Models.TaskWithTimesDto> TaskList { get; set; } = new List<Models.TaskWithTimesDto>();
        public MetaData MetaData { get; set; } = new MetaData();
        private TaskParameters _taskParameters = new TaskParameters();
        private bool successResponseTeam { get; set; } = false;

        private string LogedUserId = "";
        private string LogedUserEmail = "";
        private int UserTeamId = 0;
        private int ParticipantId = 0;
        private string UserTeamName = "";
        private int CompetitionId = 0;
        private int maxNumTasks = 0;

        protected async override Task OnInitializedAsync()
        {

            await GetUserId();
            await GetUserEmail();
            await GetUserTeam();
            await GetCompetitionId();
            await GetMaxNumTasks(); 
            await GetTasksByTeamId();

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
            successResponseParticipant = pagingResponse.SuccessRequest;
            //Console.WriteLine("Participant: ");
            //Console.WriteLine(JsonSerializer.Serialize(participant));

            if(participant == null)
            {
				UserTeamName = "No team";
			}
            else if (participant.teamId != null)
            {
                UserTeamId = participant.teamId;
                ParticipantId = participant.id;
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
        private async Task SelectedPage(int page)
        {
            _taskParameters.PageNumber = page;
            await GetTasksByTeamId();
        }

        protected async Task GetTasksByTeamId()
        {
            var pagingResponse = await TaskRepo.GetTasksByTeamId(_taskParameters, UserTeamId.ToString());
            TaskList = pagingResponse.Items;
            MetaData = pagingResponse.MetaData;
            successResponse = pagingResponse.SuccessRequest;
        }

		private async Task RemoveTaskFromTeam(int taskId)
		{
			await TaskToTeamRepo.DeleteTeamTaskByTaskIdAndTeamId(taskId.ToString(), UserTeamId.ToString());
			_taskParameters.PageNumber = 1;
			await GetTasksByTeamId();
		}

        private async Task GetCompetitionId()
        {
            if (UserTeamName != "No team")
            {
                var team = await TeamRepo.GetTeamById(UserTeamId.ToString());
                if (team.CompetitionId is not null)
                {
                    CompetitionId = (int)team.CompetitionId;
                }
            }
            
        }

        private async Task GetMaxNumTasks()
        {
            if (CompetitionId > 0)
            {
                var competition = await CompetitionRepo.GetCompetitionById(CompetitionId.ToString());
                if (competition is not null)
                {
                    maxNumTasks = competition.maxTaskPerGroups;
                    //Console.WriteLine("Max possible task number = " + maxNumTasks.ToString());
                }
            }
        }

    }
}
