using SportsLeague.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsLeague.Domain.Interfaces.Services
{
    public interface IMatchLineupService
    {
        Task<MatchLineup> AddPlayerAsync(int matchId, MatchLineup lineup);

        Task<IEnumerable<MatchLineup>> GetByMatchAsync(int matchId);

        Task<IEnumerable<MatchLineup>> GetByMatchAndTeamAsync(int matchId, int teamId);

        Task DeleteAsync(int lineupId);
    }
}
