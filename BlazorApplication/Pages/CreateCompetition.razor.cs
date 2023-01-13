using BlazorApplication.Features;
using BlazorApplication.HttpRepository;
using BlazorApplication.Models;
using BlazorApplication.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazorApplication.Pages
{
	public partial class CreateCompetition
	{
		private Competition _competition = new Competition();

		public List<CompetitionStatus> competitionStatusesList { get; set; } = new List<CompetitionStatus>();
		
		private SuccessNotification _notification;


		[Inject]
		public ICompetitionHttpRepository CompetitionRepo { get; set; }


		protected async override System.Threading.Tasks.Task OnInitializedAsync()
		{
			competitionStatusesList = (List<CompetitionStatus>)await CompetitionRepo.GetAllCompetitionStatuses();
		}

		private async void Create()
		{
			await CompetitionRepo.CreateCompetition(_competition);
			_notification.Show();
		}

	}
}
