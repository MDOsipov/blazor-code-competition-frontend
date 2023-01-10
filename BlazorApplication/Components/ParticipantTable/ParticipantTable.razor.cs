using BlazorApplication.Models;
using Microsoft.AspNetCore.Components;

namespace BlazorApplication.Components.ParticipantTable
{
    public partial class ParticipantTable
    {
		[Parameter]
		public List<Participant> Participants { get; set; }
	}
}
