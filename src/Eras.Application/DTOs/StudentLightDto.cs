using System.ComponentModel.DataAnnotations;

namespace Eras.Application.DTOs
{
    public class StudentLightDto
    {
        [Range(0, int.MaxValue, ErrorMessage = "Student Id must be zero or greater.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Student name is required.")]
        [StringLength(254, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 254 characters.")]
        public string Name { get; set; } = string.Empty;
    }
}