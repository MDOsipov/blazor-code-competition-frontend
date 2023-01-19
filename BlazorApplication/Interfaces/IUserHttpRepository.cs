using BlazorApplication.Features;
using BlazorApplication.Models;

namespace BlazorApplication.Interfaces
{
	public interface IUserHttpRepository
	{
		Task<PagingResponse<UserDto>> GetUsersExtended(UserParameters userParameters);

	}
}
