using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ABDMatrix_WebAPI.Models
{
    public class Material
    {
        [StringLength(50,ErrorMessage ="Material name can be more than 50 characters")]
        public string MatName { get; set; }

        [Required]
        public double E12 { get; set; }

        [Required]
        public double E21 { get; set; }

        [Required]
        public double G { get; set; }

        [Required]
        public double nu12 { get; set; }

        public bool IsStandard { get; set; }
    }
}
