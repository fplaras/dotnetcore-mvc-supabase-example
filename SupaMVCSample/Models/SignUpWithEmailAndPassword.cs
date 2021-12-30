namespace SupaMVCSample.Models
{
    public class SignUpWithEmailAndPassword
    {
        public string Email { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public string UserName { get { return Email; } }
        public string DisplayName
        {
            get
            {
                return String.IsNullOrEmpty(GivenName) ? UserName : String.Format("{0} {1}", GivenName, Surname);
            }
        }
        public bool IsActive { get; set; }
        public bool AdminRole { get; set; }
        public bool StaffRole { get; set; }
        public bool VolunteerRole { get; set; }
    }
}
