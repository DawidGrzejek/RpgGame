namespace RpgGame.WebApi.DTOs.Requests
{
    public class AttackRequest
    {
        public Guid AttackerId { get; set; }
        public Guid DefenderId { get; set; }
    }
}
