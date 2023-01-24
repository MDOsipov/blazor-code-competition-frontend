using BlazorApplication.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorApplication.Pages
{
	public partial class TeamManagement
	{
		[Parameter]
		public string id { get; set; } = "";

		private string teamName = "";

		[Inject]
		public ITeamHttpRepository teamRepo { get; set; }

		protected async override Task OnInitializedAsync()
		{
			var team = await teamRepo.GetTeamById(id);
			teamName = team.TeamName;
		}
	}
}
