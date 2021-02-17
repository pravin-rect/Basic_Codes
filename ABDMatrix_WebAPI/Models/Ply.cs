using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ABDMatrix_WebAPI.Models
{
    public class Ply
    {
        public int Id { get; set; }
        public string MatName { get; set; }
        [Required]
        public double Thickness { get; set; }
        [Required]
        public double Angle { get; set; }
    }
}
