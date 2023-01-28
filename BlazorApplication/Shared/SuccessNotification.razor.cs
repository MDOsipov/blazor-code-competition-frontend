using Microsoft.AspNetCore.Components;

namespace BlazorApplication.Shared
{
	public partial class SuccessNotification
	{
		[Parameter]
		public string _navUrl { get; set; } = string.Empty;

		private string _modalDisplay = string.Empty;
		private string _modalClass = string.Empty;
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
			Navigation.NavigateTo(_navUrl);
		}
	}
}
