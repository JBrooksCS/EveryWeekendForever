using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LocalShowsOnly.Models
{
    public class Event
    {
        [Key]
        [Required]
        public int id { get; set; }
        [ForeignKey("User")]
        [Required]
        public int hostId { get; set; }
        [Required]
        public string title { get; set; }
        [Required]
        public string lineup { get; set; }
        [Required]
        public string venue { get; set; }
        [Required]
        public DateTime showtime { get; set; }
        [Required]
        public string FBLink { get; set; }

    }
}
