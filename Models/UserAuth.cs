using System.Security.Claims;

namespace boxtureAssignment.Models
{
    public class UserAuth
    {
        public Guid userid { get; set; }
        public string userAuth { get; set; }
        public string passAuth { get; set; }

    }
    public class inputsAuth
    {
        public string userAuth { get; set; }
        public string passAuth { get; set; }

    }

}
