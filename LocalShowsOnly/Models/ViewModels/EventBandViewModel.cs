using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LocalShowsOnly.Models.ViewModels
{
    public class EventBandViewModel
    {
        [Key]
        [Required]
        public int id { get; set; }
        public List<Event> Events { get; set; }
        public List<Band> Bands { get; set; }
    }
}
