using System.ComponentModel.DataAnnotations;

namespace carRentalApp.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string PasswordHash { get; set; } // Password will be stored as hash
        public string Role { get; set; } // admin or user
    }
}