namespace SportsLeague.API.DTOs.Response
{
    public class TournamentSponsorResponseDTO
    {
        public int Id { get; set; }
        //Requerimientos a mostrar en el swagger
        public int TournamentId { get; set; }          
        public string TournamentName { get; set; } = ""; 
        public int SponsorId { get; set; }
        public string SponsorName { get; set; } = string.Empty;

        public decimal ContractAmount { get; set; }
        //Se declara el joinedAt como para la salida Correcta
        public DateTime JoinedAT { get; set; }
    }
}
