using BlazorApplication.Features;
using BlazorApplication.HttpRepository;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Dynamic;
using System.Text.Json;
using Task = System.Threading.Tasks.Task;

namespace BlazorApplication.Pages
{
    public partial class AllSubmittedTasks
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
        public ITaskHttpRepository TaskRepo { get; set; }

		[Inject]
		public ICompetitionHttpRepository CompetitionRepo { get; set; }

		[Inject]
        public ILogger<MySubmittedTasks> Logger { get; set; }

        [Parameter]
        public bool successResponse { get; set; } = false;
        public List<SubmittedTaskDto> TaskList { get; set; } = new List<SubmittedTaskDto>();
		public List<Team> TeamList { get; set; } = new List<Team>();

		public MetaData MetaData { get; set; } = new MetaData();
        private TaskParameters _taskParameters = new TaskParameters();
        private List<Competition> _runningCompetitions = new List<Competition>();

        private string LogedUserId = "";
        private string LogedUserEmail = "";
        private int ParticipantId = 0;
        private ErrorBoundary? errorBoundary;
        private int CurrentRunningCompetitionId { get; set; } = 0;
		private int CurrentTeamId { get; set; } = 0;


		protected async override Task OnInitializedAsync()
        {
            await GetUserId();
            await GetUserEmail();
            await GetRunningCompetitions();
            await GetTeamsByCompetitionId();
			await GetAllSubmittedTasksByCompetitionId();
        }
        protected override void OnParametersSet()
        {
            errorBoundary?.Recover();
        }
        private void ResetError()
        {
            errorBoundary?.Recover();
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
                throw new Exception("Oops! Something went wrong while getting a user info!", ex);
            }
        }

        private async Task GetRunningCompetitions()
        {
			Logger.LogInformation("Get running competitions method is called");
			CompetitionParameters competitionParameters = new CompetitionParameters()
			{
				switchOff = true
			};

			try
			{
				var pagingResponse = await CompetitionRepo.GetRunningCompetitionsByAdminId(LogedUserId, competitionParameters);
				var competitions = pagingResponse.Items;
                _runningCompetitions = competitions;
                CurrentRunningCompetitionId = _runningCompetitions.FirstOrDefault().id;
				Logger.LogInformation($"Success. Running competitions: {JsonSerializer.Serialize(_runningCompetitions)}");
			}
			catch (Exception ex)
			{
				Logger.LogError($"Error: {ex}");
				throw new Exception("Oops! Something went wrong while getting running competitions attached to you!", ex);
			}
		}

        private async Task GetTeamsByCompetitionId()
        {
			Logger.LogInformation("Get teams by competition id method is called");
			TeamParameters teamParameters = new TeamParameters()
			{
				switchOff = true
			};

			try
			{
				var pagingResponse = await TeamRepo.GetTeamsByCompetitionId(teamParameters, CurrentRunningCompetitionId.ToString());
				var teams = pagingResponse.Items;
				TeamList = teams;
				CurrentTeamId = TeamList.FirstOrDefault().Id;
				Logger.LogInformation($"Success. Teams: {JsonSerializer.Serialize(TeamList)}");
			}
			catch (Exception ex)
			{
				Logger.LogError($"Error: {ex}");
				throw new Exception("Oops! Something went wrong while getting teams!", ex);
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
                throw new Exception("Oops! Something went wrong while getting a user info!", ex);
            }
        }

        
        private async Task SelectedPage(int page)
        {
            _taskParameters.PageNumber = page;
            await GetAllSubmittedTasksByCompetitionId();
        }

		async Task HandleMyPropertyChangeCompetition(ChangeEventArgs evt)
		{
			Logger.LogInformation("HandleMyPropertyChangeCompetition method is called");
			CurrentRunningCompetitionId = Int32.Parse(evt.Value.ToString());
            await GetTeamsByCompetitionId();
			await GetAllSubmittedTasksByCompetitionId();
			StateHasChanged();
		}

		async Task HandleMyPropertyChangeTeam(ChangeEventArgs evt)
		{
			Logger.LogInformation("HandleMyPropertyChangeTeam method is called");
			CurrentTeamId = Int32.Parse(evt.Value.ToString());
			await GetAllSubmittedTasksByCompetitionId();
			StateHasChanged();
		}

		protected async Task GetAllSubmittedTasksByCompetitionId()
        {
            Logger.LogInformation("Get all submitted tasks method is called");
            try
            {
                var pagingResponse = await TaskRepo.GetAllSubmittedTasksByCompetitionIdAndTeamId(_taskParameters, CurrentRunningCompetitionId.ToString(), CurrentTeamId.ToString());
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
    }
}
