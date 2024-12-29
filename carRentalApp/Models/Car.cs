using System.ComponentModel.DataAnnotations;

namespace carRentalApp.Models
{
    public class Car
    {
        [Key]
        public int CarId { get; set; }
        [Required]
        public string CarName { get; set; }
        [Required]
        public string PlateNumber { get; set; }
        public decimal ActiveWorkingTime { get; set; }
        public decimal MaintenanceTime { get; set; }
        public decimal IdleTime { get; set; }
    }
}