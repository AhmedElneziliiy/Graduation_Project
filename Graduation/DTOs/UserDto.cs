using System.ComponentModel.DataAnnotations;

namespace Graduation.DTOs
{
    public class UserRegisterDto
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Token { get; set; }
        //public string PhotoUrl { get; set; }
        public string Gender { get; set; }
    }

    public class UserLoginDto
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Token { get; set; }
        public string PhotoUrl { get; set; }
        public string Gender { get; set; }
    }
}
