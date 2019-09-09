using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LocalShowsOnly.Models
{
    public class RSVP
    {
        [Key]
        [Required]
        public int id { get; set; }
        [ForeignKey("Event")]
        public int eventId { get; set; }
        [ForeignKey("User")]
        public int attendeeId { get; set; }
        [Required]
        public string reviewText { get; set; }
    }
}
