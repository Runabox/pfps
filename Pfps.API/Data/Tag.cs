using System.ComponentModel.DataAnnotations.Schema;

namespace Pfps.API.Data
{
    public class Tag
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}