using Microsoft.AspNetCore.Components;

namespace BlazorApplication.Pages
{
	public partial class ReportError
	{
		[Parameter]
		public int ErrorCode { get; set; }
		[Parameter]
		public string ErrorDescription { get; set; }	
	}
}
