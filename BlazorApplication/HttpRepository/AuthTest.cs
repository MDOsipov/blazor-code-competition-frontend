using BlazorApplication.Pages;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Json;

namespace BlazorApplication.HttpRepository
{
	public class AuthTest
	{
		private readonly AuthenticationStateProvider _authenticationStateProvider;
		private readonly ILogger<AuthTest> _logger;	
		public AuthTest(ILogger<AuthTest> logger, AuthenticationStateProvider authenticationStateProvider)
		{
			_logger = logger;
			_authenticationStateProvider = authenticationStateProvider;
		}

		public async Task<IIdentity> GetIdentity()
		{
            _logger.LogInformation("Get identity http repository method is called");
            try
            {
                var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;
                _logger.LogInformation($"Success. User identity: {user.Identity}");
                return user.Identity;
            }
            catch (Exception ex)
			{
                _logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while getting user info!", ex);
            }
        }

		public async Task<IEnumerable<Claim>> GetClaims()
		{
            _logger.LogInformation("Get claims http repository method is called");
            try
            {
                var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
                var claims = authState.User.Claims;
                _logger.LogInformation($"Success. Claims: {claims}");
                return claims;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while getting user info!", ex);
            }
        }
	}
}
