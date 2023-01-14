using BlazorApplication.Models;
using BlazorApplication.Pages;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorApplication.Components.ParticipantTable
{
    public partial class ParticipantTable
    {
        [Inject]
        public IJSRuntime Js { get; set; }
        [Parameter]
		public List<Participant> Participants { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Parameter]
        public EventCallback<int> OnDeleted { get; set; }

        private void RedirectToUpdate(int id)
        {
            var url = Path.Combine("/updateParticipant/", id.ToString());
            NavigationManager.NavigateTo(url);
        }

        private async System.Threading.Tasks.Task Delete(int id)
        {
            var participant = Participants.FirstOrDefault(p => p.id.Equals(id));
            var confirmed = await Js.InvokeAsync<bool>("confirm", $"Are you sure you want to delete {participant.email} participant?");

            if (confirmed)
            {
                await OnDeleted.InvokeAsync(id);
            }
        }
    }
}
