using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity;

namespace Graduation.Entities
{
    public class AppUserRole: IdentityUserRole<int>
    {
        public AppUser User { get; set; }
        public AppRole Role { get; set; }
    }
}
