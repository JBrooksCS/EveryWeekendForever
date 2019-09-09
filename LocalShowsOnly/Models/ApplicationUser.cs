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

        public virtual ICollection<Event> HostedEvents { get; set; }
        public virtual ICollection<RSVP> Attending { get; set; }
    }
}
