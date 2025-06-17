namespace RpgGame.WebApi.DTOs.Requests
{
    public class RandomEncounterRequest
    {
        public Guid CharacterId { get; set; }
        public string LocationName { get; set; }
    }
}
