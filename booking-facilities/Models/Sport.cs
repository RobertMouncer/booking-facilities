using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace booking_facilities.Models
{
    public class Sport
    {
        [Required]
        public virtual int SportId { get; set; }
        [Required]
        [MaxLength(50)]
        [DisplayName("Sport")]
        public virtual String SportName { get; set; }
    }
}
