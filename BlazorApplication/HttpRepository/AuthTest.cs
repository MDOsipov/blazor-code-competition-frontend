using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Security.Principal;

namespace BlazorApplication.HttpRepository
{
	public class AuthTest
	{
		private readonly AuthenticationStateProvider _authenticationStateProvider;

		public AuthTest()
		{

		}

		public AuthTest(AuthenticationStateProvider authenticationStateProvider)
		{
			_authenticationStateProvider = authenticationStateProvider;
		}

		public async Task<IIdentity> GetIdentity()
		{
			var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
			var user = authState.User;
			return user.Identity;
		}

		public async Task<IEnumerable<Claim>> GetClaims()
		{
			var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
			var claims = authState.User.Claims;
			return claims;
		}
	}
}
