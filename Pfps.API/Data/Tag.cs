using System.ComponentModel.DataAnnotations;

namespace Pfps.API.Data
{
    public class Tag
    {
        public Tag(string name = null) { Name = name; }

        [Key]
        public string Name { get; set; }
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    }
}