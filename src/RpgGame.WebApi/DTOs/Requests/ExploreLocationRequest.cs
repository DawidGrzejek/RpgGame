namespace RpgGame.WebApi.DTOs.Requests
{
    public class ExploreLocationRequest
    {
        public Guid CharacterId { get; set; }
        public string LocationName { get; set; }
    }
}
