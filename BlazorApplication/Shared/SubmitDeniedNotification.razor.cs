using Microsoft.AspNetCore.Components;

namespace BlazorApplication.Shared
{
    public partial class SubmitDeniedNotification
    {
        private string _modalDisplay;
        private string _modalClass;
        private bool _showBackdrop;

        [Inject]
        public NavigationManager Navigation { get; set; }

        public void Show()
        {
            _modalDisplay = "block";
            _modalClass = "show";
            _showBackdrop = true;
            StateHasChanged();
        }

        private void Hide()
        {
            _modalDisplay = "none";
            _modalClass = "";
            _showBackdrop = false;
            StateHasChanged();
        }
    }
}
