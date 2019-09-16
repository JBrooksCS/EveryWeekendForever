using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LocalShowsOnly.Models
{
    public class Band
    {
        [Key]
        [Required]
        public int id { get; set; }
        [Required]
        public string bandName { get; set; }
        [Required]
        public string bio { get; set; }
        
        public string externalLink { get; set; }
        [Required]
        public string genre { get; set; }
        [Required]
        public string photoURL { get; set; }
        public string linkToMusic { get; set; }
        [Required]
        public bool isActive { get; set; }
        [Required]
        public string hostId { get; set; }


    }
}
