using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Services
{
    public interface ISponsorService
    {
        Task<IEnumerable<Sponsor>> GetAllAsync();
        Task<Sponsor?> GetByIdAsync(int id);
        Task<Sponsor> CreateAsync(Sponsor sponsor);
        Task UpdateAsync(int id, Sponsor sponsor);
        Task DeleteAsync(int id);
        Task<IEnumerable<TournamentSponsor>> GetSponsorsByTournamentAsync(int tournamentId);
        Task<TournamentSponsor> RegisterSponsorAsync(int tournamentId, int sponsorId, decimal contractAmount);
        Task RemoveSponsorAsync(int tournamentId, int sponsorId);
    }
}
