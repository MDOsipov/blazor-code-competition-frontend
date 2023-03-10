using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using BlazorApplication.Pages;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;
using Task = System.Threading.Tasks.Task;

namespace BlazorApplication.Components.CompetitionTable
{
    public partial class CompetitionTable
    {
        private IEnumerable<UserDto> _users = new List<UserDto>();    
        [Inject]
        public IJSRuntime Js { get; set; }
     
        [Parameter]
        public EventCallback<int> OnDeleted { get; set; }

        [Parameter]
        public List<Competition> Competitions { get; set; }

        [Parameter]
        public bool IsCompetitionAdmin { get; set; } = false;

        [Inject]
        public NavigationManager NavigationManager { get; set; }

		[Inject]
		public IUserHttpRepository UserRepo { get; set; }

		protected async override System.Threading.Tasks.Task OnInitializedAsync()
        {
            await GetUsers();
            GetAdministratorsEmails();
        }

        private async Task GetUsers()
        {
            var userParameters = new UserParameters()
            {
                switchOff = true
            };
            var pagingResponse = await UserRepo.GetUsersExtended(userParameters);
            _users = pagingResponse.Items;

            Console.WriteLine("Users: " + JsonSerializer.Serialize(_users));
        }

        private void GetAdministratorsEmails()
        {
            if (Competitions.Count > 0)
            {
                Console.WriteLine("I am here!");
                foreach (var competition in Competitions)
                {
                    competition.CompetitionAdministratorEmail = _users.Where(u => u.Id == competition.CompetitionAdministratorId).Select(u => u.Email).FirstOrDefault();
                }
            }
        }

		private void RedirectToUpdate(int id)
        {
            string boolString = IsCompetitionAdmin ? "1" : "0";
            var url = Path.Combine("/updateCompetition/", id.ToString());
            NavigationManager.NavigateTo(url + "/" + boolString);
        }

		private void RedirectToManagePage(int id)
		{
			var url = Path.Combine("/competitionManagement/", id.ToString());
			NavigationManager.NavigateTo(url);
		}

		private async System.Threading.Tasks.Task Delete(int id)
        {
            var competition = Competitions.FirstOrDefault(p => p.id.Equals(id));
            var confirmed = await Js.InvokeAsync<bool>("confirm", $"Are you sure you want to delete {competition.CompetitionName} competition?");

            if (confirmed)
            {
                await OnDeleted.InvokeAsync(id);
            }

            await GetUsers();
            GetAdministratorsEmails();
        }
    }
}
