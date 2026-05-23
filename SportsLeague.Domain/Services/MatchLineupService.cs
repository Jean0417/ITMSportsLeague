using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.Domain.Services
{
    public class MatchLineupService : IMatchLineupService
    {
        private readonly IMatchLineupRepository _matchLineupRepository;
        private readonly IMatchRepository _matchRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly ILogger<MatchLineupService> _logger;

        public MatchLineupService(
            IMatchLineupRepository matchLineupRepository,
            IMatchRepository matchRepository,
            IPlayerRepository playerRepository,
            ILogger<MatchLineupService> logger)
        {
            _matchLineupRepository = matchLineupRepository;
            _matchRepository = matchRepository;
            _playerRepository = playerRepository;
            _logger = logger;
        }

        public async Task<MatchLineup> AddPlayerAsync(int matchId, MatchLineup lineup)
        {
            // =========================
            // VALIDACIÓN V1
            // Verificar que el partido exista
            // =========================
            Match? match = await _matchRepository.GetByIdAsync(matchId);

            if (match is null)
            {
                throw new KeyNotFoundException(
                    $"No se encontró el partido con ID {matchId}");
            }

            // =========================
            // VALIDACIÓN V2
            // Verificar que el jugador exista
            // =========================
            Player? player = await _playerRepository.GetByIdAsync(lineup.PlayerId);

            if (player is null)
            {
                throw new KeyNotFoundException(
                    $"No se encontró el jugador con ID {lineup.PlayerId}");
            }

            // =========================
            // VALIDACIÓN V3
            // El jugador debe pertenecer
            // a uno de los equipos del partido
            // =========================
            bool belongsToHomeTeam = player.TeamId == match.HomeTeamId;
            bool belongsToAwayTeam = player.TeamId == match.AwayTeamId;

            if (!belongsToHomeTeam && !belongsToAwayTeam)
            {
                throw new InvalidOperationException(
                    "El jugador no pertenece a ninguno de los equipos del partido");
            }

            // =========================
            // VALIDACIÓN V4
            // Evitar jugadores duplicados
            // =========================
            bool playerAlreadyExists = await _matchLineupRepository
                .ExistsByMatchAndPlayerAsync(matchId, lineup.PlayerId);

            if (playerAlreadyExists)
            {
                throw new InvalidOperationException(
                    "El jugador ya está registrado en la alineación de este partido");
            }

            // =========================
            // VALIDACIÓN V5
            // Máximo 11 titulares
            // =========================
            if (lineup.IsStarter)
            {
                int startersRegistered = await _matchLineupRepository
                    .CountStartersByMatchAndTeamAsync(matchId, player.TeamId);

                bool limitReached = startersRegistered >= 11;

                if (limitReached)
                {
                    throw new InvalidOperationException(
                        "El equipo ya tiene 11 titulares registrados en este partido");
                }
            }

            // =========================
            // VALIDACIÓN V6
            // El partido debe estar Scheduled
            // =========================
            bool matchIsScheduled = match.Status == MatchStatus.Scheduled;

            if (!matchIsScheduled)
            {
                throw new InvalidOperationException(
                    "Solo se pueden registrar alineaciones en partidos Scheduled");
            }


            // Asignar MatchId automáticamente
            lineup.MatchId = matchId;

            // Log informativo
            _logger.LogInformation(
                "Registering player {PlayerId} in match {MatchId}",
                lineup.PlayerId,
                matchId);

            // Guardar alineación
            MatchLineup createdLineup =
                await _matchLineupRepository.CreateAsync(lineup);

            return createdLineup;
        }

        public async Task<IEnumerable<MatchLineup>> GetByMatchAsync(int matchId)
        {
            bool matchExists = await _matchRepository.ExistsAsync(matchId);

            if (!matchExists)
            {
                throw new KeyNotFoundException(
                    $"No se encontró el partido con ID {matchId}");
            }

            IEnumerable<MatchLineup> lineup =
                await _matchLineupRepository.GetByMatchAsync(matchId);

            return lineup;
        }

        public async Task<IEnumerable<MatchLineup>> GetByMatchAndTeamAsync(
            int matchId,
            int teamId)
        {
            bool matchExists = await _matchRepository.ExistsAsync(matchId);

            if (!matchExists)
            {
                throw new KeyNotFoundException(
                    $"No se encontró el partido con ID {matchId}");
            }

            IEnumerable<MatchLineup> teamLineup =
                await _matchLineupRepository.GetByMatchAndTeamAsync(matchId, teamId);

            return teamLineup;
        }

        public async Task DeleteAsync(int lineupId)
        {
            bool lineupExists =
                await _matchLineupRepository.ExistsAsync(lineupId);

            if (!lineupExists)
            {
                throw new KeyNotFoundException(
                    $"No se encontró el registro de alineación con ID {lineupId}");
            }

            await _matchLineupRepository.DeleteAsync(lineupId);

            _logger.LogInformation(
                "Lineup record {LineupId} deleted successfully",
                lineupId);
        }
    }
}
