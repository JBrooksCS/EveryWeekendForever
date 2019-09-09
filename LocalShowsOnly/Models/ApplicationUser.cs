using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace LocalShowsOnly.Models
{
    public class ApplicationUser : IdentityUser
    {
        //[Key]
        //[Required]
        //public int id { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string userName { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string userEmail { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string firstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string lastName { get; set; }

        [Display(Name = "Profile Photo")]
        public string photoURL { get; set; }

        public virtual ICollection<Event> HostedEvents { get; set; }
        public virtual ICollection<RSVP> Attending { get; set; }
    }
}
