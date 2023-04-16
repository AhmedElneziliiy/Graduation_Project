namespace Graduation.Helpers
{
    public class UserParams : PaginationParams
    {


        public string CurrentUsername { get; set; }
        public string Gender { get; set; }
        public string OrderBy { get; set; } = "lastActive";
    }
}
