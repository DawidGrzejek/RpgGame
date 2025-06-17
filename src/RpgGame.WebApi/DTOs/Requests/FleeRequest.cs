namespace RpgGame.WebApi.DTOs.Requests
{
    public class FleeRequest
    {
        public Guid CharacterId { get; set; }
        public Guid EnemyId { get; set; }
    }
}
