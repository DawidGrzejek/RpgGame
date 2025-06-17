namespace RpgGame.WebApi.DTOs.Requests
{
    public class MoveLocationRequest
    {
        public Guid CharacterId { get; set; }
        public string LocationName { get; set; }
    }
}
