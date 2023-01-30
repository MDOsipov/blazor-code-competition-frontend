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
    public partial class MySubmittedTasks
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
        public ILogger<MySubmittedTasks> Logger { get; set; }   
        public List<Models.TaskWithTimesDto> TaskList { get; set; } = new List<Models.TaskWithTimesDto>();
        public MetaData MetaData { get; set; } = new MetaData();
        private TaskParameters _taskParameters = new TaskParameters();

        private string LogedUserId = "";
        private string LogedUserEmail = "";
        private int UserTeamId = 0;
        private int ParticipantId = 0;
        private string UserTeamName = "";
        private ErrorBoundary? errorBoundary;

        [Inject]
        public ITaskHttpRepository TaskRepo { get; set; }

        protected async override System.Threading.Tasks.Task OnInitializedAsync()
        {
            await GetUserId();
            await GetUserEmail();
            await GetUserTeam();
            await GetTasksByTeamId();
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
                Console.WriteLine("Our user id: " + LogedUserId);
                Logger.LogInformation($"Success. User id: {JsonSerializer.Serialize(LogedUserId)}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new System.Exception("Oops! Something went wrong while getting a user info!", ex);
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
                throw new System.Exception("Oops! Something went wrong while getting a user info!", ex);
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
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new System.Exception("Oops! Something went wrong while getting a user info!", ex);
            }
            
            if (participant?.teamId is not null)
            {
                UserTeamId = (int)participant.teamId;
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
                    throw new System.Exception("Oops! Something went wrong while getting a user team!", ex);
                }
            }
            else
            {
                UserTeamName = "No team";
                Logger.LogInformation($"Success. User team: {JsonSerializer.Serialize(UserTeamName)}");
            }
        }
        private async System.Threading.Tasks.Task SelectedPage(int page)
        {
            _taskParameters.PageNumber = page;
            await GetTasksByTeamId();
        }

        protected async System.Threading.Tasks.Task GetTasksByTeamId()
        {
            Logger.LogInformation("Get tasks by team id method is called");
            try
            {
                var pagingResponse = await TaskRepo.GetSubmittedTasksByTeamId(_taskParameters, UserTeamId.ToString());
                TaskList = pagingResponse.Items;
                MetaData = pagingResponse.MetaData;
                Logger.LogInformation($"Success. Tasks: {JsonSerializer.Serialize(TaskList)}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new System.Exception("Oops! Something went wrong while getting a list of tasks!", ex);
            }  
        }
    }
}

