namespace RpgGame.WebApi.DTOs.Requests
{
    public class SaveGameRequest
    {
        public Guid CharacterId { get; set; }
        public string LocationName { get; set; }
        public string SaveName { get; set; }
    }
}
