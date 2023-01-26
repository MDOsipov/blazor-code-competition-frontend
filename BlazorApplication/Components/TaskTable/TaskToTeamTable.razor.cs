using BlazorApplication.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorApplication.Components.TaskTable
{
    public partial class TaskToTeamTable
    {
        [Inject]
        public IJSRuntime Js { get; set; }

        private SubmitDeniedNotification _notification;

        [Parameter]
        public bool IsSubmitted { get; set; } = false;

        [Parameter]
        public List<Models.TaskWithTimesDto> Tasks { get; set; }
        [Parameter]
        public string TeamIdStr { get; set; } = "";

        [Parameter]
        public EventCallback<int> OnDeleted { get; set; }

        [Parameter]
        public int ParamParticipantId { get; set; } = 0;

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public Dictionary<int, string> StatusDict = new Dictionary<int, string>()
        {
            [(int)Enums.TaskStatus.Success] = "Success",
            [(int)Enums.TaskStatus.Fail] = "Fail",
			[(int)Enums.TaskStatus.PartialSuccess] = "Partial success",
			[(int)Enums.TaskStatus.Submitted] = "Submitted"
		};

		private async void RedirectToSubmit(int participantId, int taskId)
		{
            if (ParamParticipantId == participantId)
            {
                var url = Path.Combine("/submitTask/" + taskId.ToString() + "/" + TeamIdStr);
                Console.WriteLine("Url - " + url);
                NavigationManager.NavigateTo(url);
            }
            else
            {
                _notification.Show();
            }
        }
		private async Task Delete(int id)
        {
            var task = Tasks.FirstOrDefault(p => p.Id.Equals(id));
            var confirmed = await Js.InvokeAsync<bool>("confirm", $"Are you sure you want to remove {task.TaskName} from your list of tasks?");

            if (confirmed)
            {
                await OnDeleted.InvokeAsync(id);
            }
        }
    }
}
