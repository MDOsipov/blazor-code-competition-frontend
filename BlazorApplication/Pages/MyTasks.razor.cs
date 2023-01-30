using BlazorApplication.Features;
using BlazorApplication.HttpRepository;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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

        [Inject]
        public ILogger<MyTasks> Logger { get; set; }

        [Parameter]
        public bool successResponse { get; set; } = false;

        [Parameter]
        public bool successResponseParticipant { get; set; } = false;
		public List<TaskWithTimesDto> TaskList { get; set; } = new List<TaskWithTimesDto>();
        public MetaData MetaData { get; set; } = new MetaData();
        private TaskParameters _taskParameters = new TaskParameters();
        private bool successResponseTeam { get; set; } = false;
        private ErrorBoundary? errorBoundary;
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
            Logger.LogInformation("Get user id method is called");
            try
            {
                var claims = await AuthTest.GetClaims();
                LogedUserId = claims.Where(c => c.Type == "sub").FirstOrDefault().Value.ToString();
                Logger.LogInformation($"Success. User id: {JsonSerializer.Serialize(LogedUserId)}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new System.Exception("Oops! Something went wrong while getting user info!", ex);
            }
        }

        private async Task GetUserEmail()
        {
            Logger.LogInformation("Get user email method is called");
            UserParameters userParameters = new UserParameters()
            {
                switchOff = true
            };

            try
            {
                var pagingResponse = await UserRepo.GetUsersExtended(userParameters);
                var users = pagingResponse.Items;
                LogedUserEmail = users.Where(u => u.Id == LogedUserId).FirstOrDefault().Email;
                Logger.LogInformation($"Success. User email: {JsonSerializer.Serialize(LogedUserEmail)}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while getting user info!", ex);
            }
        }

        private async Task GetUserTeam()
        {
            Logger.LogInformation("Get user team method is called");
            ParticipantParameters participantParameters = new ParticipantParameters()
            {
                switchOff = true
            };

            Participant participant;
            try
            {
                var pagingResponse = await ParticipantRepo.GetParticipantsByEmail(participantParameters, LogedUserEmail);
                participant = pagingResponse.Items.FirstOrDefault();
                successResponseParticipant = true;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while getting user info!", ex);
            }


            if (participant == null)
            {
                UserTeamName = "No team";
            }
            else if (participant.teamId != null)
            {
                UserTeamId = participant.teamId;
                ParticipantId = participant.id;
            }

            if (UserTeamId > 0)
            {
                try
                {
                    var team = await TeamRepo.GetTeamById(UserTeamId.ToString());
                    UserTeamName = team.TeamName;
                    Logger.LogInformation($"Success. User team: {JsonSerializer.Serialize(UserTeamName)}");
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error: {ex}");
                    throw new Exception("Oops! Something went wrong while getting user team!", ex);
                }
            }
            else
            {
                UserTeamName = "No team";
                Logger.LogInformation($"Success. User team: {JsonSerializer.Serialize(UserTeamName)}");
            }

        }
        private async Task SelectedPage(int page)
        {
            _taskParameters.PageNumber = page;
            await GetTasksByTeamId();
        }

        protected async Task GetTasksByTeamId()
        {
            Logger.LogInformation("Get tasks by team id method is called");
            try
            {
                var pagingResponse = await TaskRepo.GetTasksByTeamId(_taskParameters, UserTeamId.ToString());
                TaskList = pagingResponse.Items;
                MetaData = pagingResponse.MetaData;
                successResponse = true;
                Logger.LogInformation($"Success. Tasks: {JsonSerializer.Serialize(TaskList)}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while getting a list of tasks!", ex);
            }
        }

		private async Task RemoveTaskFromTeam(int taskId)
		{
            Logger.LogInformation("Remove task from team method is called");
            try
            {
                await TaskToTeamRepo.DeleteTeamTaskByTaskIdAndTeamId(taskId.ToString(), UserTeamId.ToString());
                _taskParameters.PageNumber = 1;
                Logger.LogInformation($"Success. A task is removed from the team");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while removing a task!", ex);
            }
            
			await GetTasksByTeamId();
		}

        private async Task GetCompetitionId()
        {
            Logger.LogInformation("Get competition id method is called");
            Team team;
            try
            {
                if (UserTeamName != "No team")
                {
                    team = await TeamRepo.GetTeamById(UserTeamId.ToString());
					if (team.CompetitionId is not null)
					{
						CompetitionId = (int)team.CompetitionId;
						Logger.LogInformation($"Success. Competition id: {JsonSerializer.Serialize(CompetitionId)}");
					}
				}
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while getting a user team!", ex);
            }            
        }
        protected override void OnParametersSet()
        {
            errorBoundary?.Recover();
        }
        private void ResetError()
        {
            errorBoundary?.Recover();
        }

        private async Task GetMaxNumTasks()
        {
            Logger.LogInformation("Get maximum number of tasks method is called");
            if (CompetitionId > 0)
            {
                Competition competition;
                try
                {
                    competition = await CompetitionRepo.GetCompetitionById(CompetitionId.ToString());
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error: {ex}");
                    throw new Exception("Oops! Something went wrong while getting current competition!", ex);
                }

                if (competition is not null)
                {
                    maxNumTasks = competition.maxTaskPerGroups;
                    Logger.LogInformation($"Success. Maximum number of tasks: {JsonSerializer.Serialize(maxNumTasks)}");
                }
            }
        }

    }
}
