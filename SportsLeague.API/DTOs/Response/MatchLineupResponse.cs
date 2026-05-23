namespace SportsLeague.API.DTOs.Response
{
    public class MatchLineupResponse
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public int PlayerId { get; set; }
        public string PlayerName { get; set; } = string.Empty;  // FirstName + " " + LastName
        public string TeamName { get; set; } = string.Empty;    // Player.Team.Name
        public bool IsStarter { get; set; }
        public string Position { get; set; } = string.Empty;
    }
}
