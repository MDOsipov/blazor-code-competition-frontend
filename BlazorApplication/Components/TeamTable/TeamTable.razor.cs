using BlazorApplication.Models;
using Microsoft.AspNetCore.Components;

namespace BlazorApplication.Components.TeamTable
{
	public partial class TeamTable
	{
		[Parameter]
		public List<Team> Teams { get; set; }	
	}
}
