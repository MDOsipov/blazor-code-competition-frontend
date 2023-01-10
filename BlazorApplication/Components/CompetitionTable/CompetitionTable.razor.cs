using BlazorApplication.Models;
using Microsoft.AspNetCore.Components;

namespace BlazorApplication.Components.CompetitionTable
{
    public partial class CompetitionTable
    {
        [Parameter]
        public List<Competition> Competitions { get; set; }
    }
}
