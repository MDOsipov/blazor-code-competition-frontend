using BlazorApplication.Models;
using Microsoft.AspNetCore.Components;

namespace BlazorApplication.Components.ParticipantTable
{
    public partial class ParticipantTable
    {
		[Parameter]
		public List<Participant> Participants { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        private void RedirectToUpdate(int id)
        {
            var url = Path.Combine("/updateParticipant/", id.ToString());
            NavigationManager.NavigateTo(url);
        }
    }
}
