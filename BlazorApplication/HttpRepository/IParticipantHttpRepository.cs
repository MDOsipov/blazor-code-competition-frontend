using BlazorApplication.Models;

namespace BlazorApplication.HttpRepository
{
	public interface IParticipantHttpRepository
	{
		Task<ICollection<Participant>> GetParticipants();
	}
}
