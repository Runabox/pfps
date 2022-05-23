using Pfps.API.Data;

namespace Pfps.API.Models
{
    public class TagViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime Timestamp { get; set; }

        public static TagViewModel From(Tag tag)
        {
            return new TagViewModel()
            {
                Id = tag.Id,
                Title = tag.Title,
                Timestamp = tag.Timestamp,
            };
        }
    }
}