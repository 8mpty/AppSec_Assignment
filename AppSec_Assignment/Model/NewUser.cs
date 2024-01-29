using Microsoft.AspNetCore.Identity;

namespace AppSec_Assignment.Model
{
    public class NewUser : IdentityUser
    {
        public string FirstName {  get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string NRIC {  get; set; }
        public DateTime DateofBirth { get; set; }

        public string WAI {  get; set; }

        public int Logs { get; set; }
    }
}
