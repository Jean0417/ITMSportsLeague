using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SportsLeague.API.DTOs.Request;
using SportsLeague.API.DTOs.Response;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.API.Controllers
{
    [ApiController]
    [Route("api/match/{matchId}/lineup")]
    public class MatchLineupController : ControllerBase
    {
        private readonly IMatchLineupService _matchLineupService;
        private readonly IMapper _mapper;

        public MatchLineupController(
            IMatchLineupService matchLineupService,
            IMapper mapper)
        {
            _matchLineupService = matchLineupService;
            _mapper = mapper;
        }

        // =========================================
        // POST: api/match/{matchId}/lineup
        // Agregar jugador a la alineación
        // =========================================
        [HttpPost]
        public async Task<ActionResult<MatchLineupResponse>> AddPlayer(
            int matchId,
            [FromBody] MatchLineupRequest request)
        {
            try
            {
                // Convertir DTO de entrada a entidad
                MatchLineup lineup = _mapper.Map<MatchLineup>(request);

                // Guardar alineación
                MatchLineup createdLineup =
                    await _matchLineupService.AddPlayerAsync(matchId, lineup);

                // Obtener registro completo con relaciones
                IEnumerable<MatchLineup> matchLineups =
                    await _matchLineupService.GetByMatchAsync(matchId);

                MatchLineup? lineupWithDetails = matchLineups
                    .FirstOrDefault(l => l.Id == createdLineup.Id);

                // Convertir entidad a DTO de respuesta
                MatchLineupResponse response =
                    _mapper.Map<MatchLineupResponse>(lineupWithDetails);

                // HTTP 201 Created
                return StatusCode(StatusCodes.Status201Created, response);
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(new
                {
                    message = exception.Message
                });
            }
            catch (InvalidOperationException exception)
            {
                return Conflict(new
                {
                    message = exception.Message
                });
            }
        }

        // =========================================
        // GET: api/match/{matchId}/lineup
        // Obtener alineación completa
        // =========================================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MatchLineupResponse>>> GetByMatch(
            int matchId)
        {
            try
            {
                IEnumerable<MatchLineup> lineups =
                    await _matchLineupService.GetByMatchAsync(matchId);

                IEnumerable<MatchLineupResponse> response =
                    _mapper.Map<IEnumerable<MatchLineupResponse>>(lineups);

                return Ok(response);
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(new
                {
                    message = exception.Message
                });
            }
        }

        // =========================================
        // GET: api/match/{matchId}/lineup/team/{teamId}
        // Obtener alineación por equipo
        // =========================================
        [HttpGet("team/{teamId}")]
        public async Task<ActionResult<IEnumerable<MatchLineupResponse>>> GetByTeam(
            int matchId,
            int teamId)
        {
            try
            {
                IEnumerable<MatchLineup> lineups =
                    await _matchLineupService
                        .GetByMatchAndTeamAsync(matchId, teamId);

                IEnumerable<MatchLineupResponse> response =
                    _mapper.Map<IEnumerable<MatchLineupResponse>>(lineups);

                return Ok(response);
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(new
                {
                    message = exception.Message
                });
            }
        }

        // =========================================
        // DELETE: api/match/{matchId}/lineup/{id}
        // Eliminar jugador de la alineación
        // =========================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _matchLineupService.DeleteAsync(id);

                // HTTP 204 No Content
                return NoContent();
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(new
                {
                    message = exception.Message
                });
            }
        }
    }
}