namespace SupaMVCSample.Models
{
    public class SignUpWithEmailAndPassword
    {
        public string Email { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public string UserName { get { return Email; } }
        public string Password { get; set; }
        public string DisplayName
        {
            get
            {
                return String.IsNullOrEmpty(GivenName) ? UserName : String.Format("{0} {1}", GivenName, Surname);
            }
        }
        public bool IsActive { get; set; }
        public int RoleId { get; set; }
    }
}
