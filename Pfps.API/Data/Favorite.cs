namespace Pfps.API.Data
{
    public class Favorite
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public Upload Upload { get; set; }
    }
}