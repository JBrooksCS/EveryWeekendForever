using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LocalShowsOnly.Models
{
    public class Review
    {
        [Key]
        [Required]
        public int id { get; set; }
        [Required]
        public string reviewText { get; set; }
        [Required]
        [ForeignKey("Event")]

        public int eventId { get; set; }
        [Required]
        [ForeignKey("User")]
        public int userId { get; set; }
    }
}
