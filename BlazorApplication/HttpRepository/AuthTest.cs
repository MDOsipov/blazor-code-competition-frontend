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
            try 
			{
                var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;
                return user.Identity;
            }
            catch (Exception ex)
			{
                throw new System.Exception("Oops! Something went wrong while getting user info!", ex);
            }
        }

		public async Task<IEnumerable<Claim>> GetClaims()
		{
			try 
			{
                var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
                var claims = authState.User.Claims;
                return claims;
            }
            catch (Exception ex)
            {
                throw new System.Exception("Oops! Something went wrong while getting user info!", ex);
            }
        }
	}
}
