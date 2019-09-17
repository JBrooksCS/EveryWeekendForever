﻿using System;
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
        public string hostId { get; set; }
        [Required]
        
        [StringLength(60)]
        public string title { get; set; }
        
        [Required]
        public int venueId { get; set; }
        public Venue venue { get; set; }
        
        [Required, Column(TypeName = "Date")]
        public DateTime showtime { get; set; }
        [Required]
        [RegularExpression("([0-9]+)", ErrorMessage = "Please enter valid Number")]
        //[RegularExpression(@"^[0-9a-zA-Z''-'\s]{1,40}$", ErrorMessage = "Special characters are not allowed")]
        public int price { get; set; }

        [StringLength(80)]
        public string externalLink { get; set; }
        
        public string photoURL { get; set; }
        [Required]
        public string description { get; set; }


        public virtual ICollection<RSVP> RSVPs { get; set; }
        
    }
}
