using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ABDMatrix_WebAPI.Models
{
    public class Laminate
    {
        [Required]
        public Material Material { get; set; }
        [Required]
        public IEnumerable<Ply> Plies { get; set; }
    }
}
