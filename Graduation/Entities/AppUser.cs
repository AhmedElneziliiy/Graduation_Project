using Microsoft.AspNetCore.Identity;

namespace Graduation.Entities
{
    public class AppUser: IdentityUser<int>
    {
        public DateTime DateOfBirth { get; set; }
        public string KnownAs { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime LastActive { get; set; } = DateTime.Now;
        public string Gender { get; set; }
        public List<Photo> Photos { get; set; } = new();
        public string City { get; set; }
        public string Country { get; set; }
        public ICollection<AppUserRole> UserRoles { get; set; }


        public List<Message> MessagesSent { get; set; }
        public List<Message> MessagesReceived { get; set; }
    }
}
