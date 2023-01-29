﻿using BlazorApplication.Features;
using BlazorApplication.HttpRepository;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Json;
using static Microsoft.AspNetCore.Components.Authorization.AuthenticationState;
using Task = System.Threading.Tasks.Task;

namespace BlazorApplication.Pages
{
    
    public partial class MyCompetitions
    {
		[Inject]
		public AuthTest authTest { get; set; }
		public List<Competition> CompetitionList { get; set; } = new List<Competition>();

		public MetaData MetaData { get; set; } = new MetaData();
		private CompetitionParameters _competitionParameters = new CompetitionParameters();
        private ErrorBoundary? errorBoundary;

        [Inject]
		public ICompetitionHttpRepository CompetitionRepo { get; set; }

		private string LogedUserId = "";

		protected async override System.Threading.Tasks.Task OnInitializedAsync()
		{
			await LogUsername();
			await GetUserId();
			await GetCompetitions();
		}

		protected async System.Threading.Tasks.Task GetCompetitions()
		{
			try
			{
                var pagingResponse = await CompetitionRepo.GetCompetitionsByAdminId(LogedUserId, _competitionParameters);
                CompetitionList = pagingResponse.Items;
                MetaData = pagingResponse.MetaData;
            }
            catch (Exception ex)
            {
                throw new System.Exception("Oops! Something went wrong while getting a list of competitions!", ex);
            }
		}
		private async System.Threading.Tasks.Task DeleteCompetition(int id)
		{
			try
			{
                await CompetitionRepo.DeleteCompetition(id);
                _competitionParameters.PageNumber = 1;
            }
            catch (Exception ex)
            {
                throw new System.Exception("Oops! Something went wrong while removing a competition!", ex);
            }
			await GetCompetitions();
		}
		private async System.Threading.Tasks.Task SelectedPage(int page)
		{
			_competitionParameters.PageNumber = page;
			await GetCompetitions();
		}

		private async Task LogUsername()
		{
			var identity = await authTest.GetIdentity();
			//Console.WriteLine(identity.Name);
			Console.WriteLine("Identity: ");
			Console.WriteLine(JsonSerializer.Serialize(identity));
		}

		private async Task LogClaims()
		{
			var claims = await authTest.GetClaims();

			foreach(var claim in claims)
			{
				Console.WriteLine(claim.Type + " " + claim.Value);
			}
		}

		private async Task GetUserId()
		{
			try
			{
                var claims = await authTest.GetClaims();
                LogedUserId = claims.Where(c => c.Type == "sub").FirstOrDefault().Value.ToString();
                Console.WriteLine("Our user id: " + LogedUserId);
            }
			catch(Exception ex)
			{
                throw new System.Exception("Oops! Something went wrong while getting user info!", ex);
            }
        }
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
