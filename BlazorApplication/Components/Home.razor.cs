using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Xml.Linq;

namespace BlazorApplication.Components
{
	public partial class Home
	{
		private ErrorBoundary? errorBoundary;
		[Parameter]
		public string Title { get; set; }

		[Parameter(CaptureUnmatchedValues = true)]
		public Dictionary<string, object> AdditionalAttributes { get; set; }

		[CascadingParameter(Name = "HeadingColor")]
		public string Color { get; set; }

		[Parameter]
		public RenderFragment VisitShopContent { get; set; }
		protected override void OnParametersSet()
		{
			errorBoundary?.Recover();
		}
		private void ResetError()
		{
			errorBoundary?.Recover();
		}
	}
}
