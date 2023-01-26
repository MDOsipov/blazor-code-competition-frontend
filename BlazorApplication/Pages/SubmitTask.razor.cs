using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using BlazorApplication.Shared;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace BlazorApplication.Pages
{
    public partial class SubmitTask
	{
        private SuccessfulSubmit? _notification;

        [Inject]
        public ITaskToTeamHttpRepository? TaskToTeamRepo { get; set; }

		[Parameter]
		public string taskIdStr { get; set; } = "";
        [Parameter]
        public string teamIdStr { get; set; } = "";

        private SubmitTaskDataDto _submitData = new SubmitTaskDataDto();

		private async System.Threading.Tasks.Task Submit()
		{
            if (_notification is not null && TaskToTeamRepo is not null)
            {
                await TaskToTeamRepo.SubmitTask(taskIdStr, teamIdStr, _submitData);
                _notification.Show();
            }
        }
    }
}
