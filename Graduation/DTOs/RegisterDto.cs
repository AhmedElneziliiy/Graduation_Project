using System.ComponentModel.DataAnnotations;

namespace Graduation.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required] public string FirstName{ get; set; }
        [Required] public string LastName{ get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]

        public DateTime DateOfBirth { get; set; }// optional to make required work!

        [Required]
        [StringLength(8, MinimumLength = 4)]
        public string Password { get; set; }
    }
}
