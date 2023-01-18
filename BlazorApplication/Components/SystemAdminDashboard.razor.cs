using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using Microsoft.AspNetCore.Components;

namespace BlazorApplication.Components
{
    public partial class SystemAdminDashboard
    {
		public List<Competition> CompetitionList { get; set; } = new List<Competition>();

		[Inject]
		public ICompetitionHttpRepository CompetitionRepo { get; set; } 

		protected async override System.Threading.Tasks.Task OnInitializedAsync()
		{
			await GetCompetitions();
		}

		protected async System.Threading.Tasks.Task GetCompetitions()
		{
			CompetitionParameters competitionParameters = new CompetitionParameters
			{
				switchOff = true
			};

			var pagingResponse = await CompetitionRepo.GetCompetitions(competitionParameters);
			CompetitionList = pagingResponse.Items;
		}
	}
}
