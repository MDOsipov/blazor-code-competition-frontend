using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace BlazorApplication.Shared
{
    public partial class AccessControl
    {
        [Inject]
        public SignOutSessionStateManager SignOutManager { get; set; }
        public NavigationManager Navigation { get; set; }

        private async Task BeginSignOut(MouseEventArgs args)
        {
            await SignOutManager.SetSignOutState();
        }
    }
}
