using Microsoft.AspNetCore.Components;

namespace BlazorApplication.Pages
{
    public partial class Authentication
    {
        [Parameter]
        public string Action { get; set; }
        
        [Inject]
        private IConfiguration Configuration { get; set; }

        [Inject]
        private NavigationManager Navigation { get; set; }

        private void Logout()
        {
            var authority = Configuration["Auth0:Authority"];
            var clientId = Configuration["Auth0:ClientId"];
            Navigation.NavigateTo($"{authority}/v2/logout?client_id={clientId}");
        }
    }
}
