using System.ComponentModel.DataAnnotations;

namespace boxtureAssignment.Controllers.Entity
{
    public class userEntity
    {
        public Guid Id { get; set; }
        [Required]
        public string username { get; set; }
        [Required]
        public string password { get; set; }
        public bool isAdmin { get; set; }
        [Required]
        public int age { get; set; }

        [Required]
        public string[] hobbies { get; set; }
    }
}
