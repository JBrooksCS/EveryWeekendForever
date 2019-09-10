using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LocalShowsOnly.Models
{
    public class Venue
    {
        [Key]
        [Required]
        public int id { get; set; }
        [Required]
        public string venueName { get; set; }
        [Required]
        public string venueAddress { get; set; }
        [Required]
        public string venueDetails { get; set; }
        [Required]
        public string phoneNumber { get; set; }
        [Required]
        public string email { get; set; }
        [Required]
        public string photoURL { get; set; }
        [Required]
        public string websiteURL { get; set; }

        public virtual ICollection<Event> VenueEvents { get; set; }

    }
}
