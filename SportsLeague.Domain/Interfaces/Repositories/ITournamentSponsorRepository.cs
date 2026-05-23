using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Repositories
{
    public interface ITournamentSponsorRepository : IGenericRepository<TournamentSponsor>
    {
        Task<TournamentSponsor?> GetByTournamentAndSponsorAsync(int tournamentId, int sponsorId);
        Task<IEnumerable<TournamentSponsor>> GetSponsorsByTournamentAsync(int tournamentId);
        Task<TournamentSponsor> CreateWithIncludesAsync(TournamentSponsor entity);
    }
}
