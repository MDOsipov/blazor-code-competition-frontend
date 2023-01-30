using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using BlazorApplication.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace BlazorApplication.Pages
{
    public partial class SubmitTask
	{
        private SuccessfulSubmit? _notification;

        [Inject]
        public ITaskToTeamHttpRepository? TaskToTeamRepo { get; set; }
        
        [Inject]
        public ILogger<SubmitTask> Logger { get; set; }
		[Parameter]
		public string taskIdStr { get; set; } = "";
        [Parameter]
        public string teamIdStr { get; set; } = "";
        private ErrorBoundary? errorBoundary;

        private SubmitTaskDataDto _submitData = new SubmitTaskDataDto();

		private async System.Threading.Tasks.Task Submit()
		{
            Logger.LogInformation("Submit method is called");
            if (_notification is not null && TaskToTeamRepo is not null)
            {
                try
                {
                    await TaskToTeamRepo.SubmitTask(taskIdStr, teamIdStr, _submitData);
                    Logger.LogInformation($"Success. The task is successfully submitted");
                    _notification.Show();
                }
                catch(Exception ex)
                {
                    Logger.LogError($"Error: {ex}");
                    throw new Exception("Oops! Something went wrong while submitting a task!", ex);
                }
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
    }
}
