using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SupaMVCSample.Enums
{
    public enum RolesEnum
    {
        [Display(Name = "Adminitrator")]
        Admin = 1,
        [Display(Name = "Volunteer Member")]
        Volunteer = 2,
        [Display(Name = "Staff Member")]
        Member = 3
    }
}
