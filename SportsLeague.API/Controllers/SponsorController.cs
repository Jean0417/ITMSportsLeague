using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SportsLeague.API.DTOs.Request;
using SportsLeague.API.DTOs.Response;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SponsorController : ControllerBase
    {
        private readonly ISponsorService _sponsorService;
        private readonly IMapper _mapper;

        public SponsorController(ISponsorService sponsorService, IMapper mapper)
        {
            _sponsorService = sponsorService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SponsorResponseDTO>>> GetAll()
        {
            IEnumerable<Sponsor> sponsors = await _sponsorService.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<SponsorResponseDTO>>(sponsors));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SponsorResponseDTO>> GetById(int id)
        {
            Sponsor? sponsor = await _sponsorService.GetByIdAsync(id);
            if (sponsor == null)
                return NotFound();

            return Ok(_mapper.Map<SponsorResponseDTO>(sponsor));
        }

        [HttpPost]
        public async Task<ActionResult<SponsorResponseDTO>> Create(SponsorRequestDTO dto)
        {
            try
            {
                Sponsor entity = _mapper.Map<Sponsor>(dto);
                Sponsor created = await _sponsorService.CreateAsync(entity);

                SponsorResponseDTO response = _mapper.Map<SponsorResponseDTO>(created);

                return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, SponsorRequestDTO dto)
        {
            try
            {
                Sponsor entity = _mapper.Map<Sponsor>(dto);
                await _sponsorService.UpdateAsync(id, entity);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _sponsorService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("{id}/tournaments")]
        public async Task<ActionResult<IEnumerable<TournamentSponsorResponseDTO>>> GetTournaments(int id)
        {
            try
            {
                var links = await _sponsorService.GetSponsorsByTournamentAsync(id);
                return Ok(_mapper.Map<IEnumerable<TournamentSponsorResponseDTO>>(links));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/tournaments")]
        public async Task<IActionResult> RegisterSponsor(int id, TournamentSponsorRequestDTO dto)
        {
            try
            {
                var result = await _sponsorService.RegisterSponsorAsync(id, dto.SponsorId, dto.ContractAmount);

                var response = _mapper.Map<TournamentSponsorResponseDTO>(result);

                return CreatedAtAction(nameof(GetTournaments), new { id = id }, response);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // Delete
        [HttpDelete("{id}/tournaments/{tid}")]
        public async Task<IActionResult> RemoveTournament(int id, int tid)
        {
            try
            {
                await _sponsorService.RemoveSponsorAsync(id, tid);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}