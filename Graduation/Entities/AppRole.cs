using Microsoft.AspNetCore.Identity;

namespace Graduation.Entities
{
    public class AppRole:IdentityRole<int>
    {
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}
