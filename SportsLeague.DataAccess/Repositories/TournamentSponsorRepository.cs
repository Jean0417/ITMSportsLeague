using Microsoft.EntityFrameworkCore;
using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;

namespace SportsLeague.DataAccess.Repositories
{
    public class TournamentSponsorRepository
        : GenericRepository<TournamentSponsor>, ITournamentSponsorRepository
    {
        public TournamentSponsorRepository(LeagueDbContext context) : base(context)
        {
        }

        public async Task<TournamentSponsor?> GetByTournamentAndSponsorAsync(int tournamentId, int sponsorId)
        {
            return await _dbSet
                .Where(ts => ts.TournamentId == tournamentId && ts.SponsorId == sponsorId)
                .Include(ts => ts.Sponsor)
                .Include(ts => ts.Tournament)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TournamentSponsor>> GetSponsorsByTournamentAsync(int tournamentId)
        {
            return await _dbSet
             .Where(ts => ts.TournamentId == tournamentId)
             .Include(ts => ts.Sponsor)      
             .Include(ts => ts.Tournament)   
             .AsNoTracking()                 
             .ToListAsync();
        }
        public async Task<TournamentSponsor> CreateWithIncludesAsync(TournamentSponsor entity)
        {
            entity.CreatedAt = DateTime.UtcNow;

            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();

            return await _dbSet
                .Where(ts => ts.TournamentId == entity.TournamentId && ts.SponsorId == entity.SponsorId)
                .Include(ts => ts.Tournament)
                .Include(ts => ts.Sponsor)
                .AsNoTracking()
                .FirstAsync();
        }
    }
}
