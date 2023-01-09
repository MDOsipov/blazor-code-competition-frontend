using BlazorApplication.Models;

namespace BlazorApplication.HttpRepository
{
    public interface ICompetitionHttpRepository
    {
        Task<ICollection<Competition>> GetCompetitions();
    }
}
