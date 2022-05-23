using System.ComponentModel.DataAnnotations;

namespace Pfps.API.Models
{
    public class ApiUserModel
    {
        [MinLength(3)]
        [MaxLength(16)]
        [Required]
        public string Username { get; set; }

        [MinLength(6)]
        [MaxLength(32)]
        [Required]
        public string Password { get; set; }

        [EmailAddress]
        [MaxLength(256)]
        [Required]
        public string Email { get; set; }
    }
}