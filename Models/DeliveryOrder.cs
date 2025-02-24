using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScanBarcode.Models
{
    public class DeliveryOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Doid { get; set; }

        [Required]
        public string Donumber { get; set; } 

        public string? SONumber { get; set; }

        public DateTime RequestedDate { get; set; }

        [Required]
        public int ModelId { get; set; }

        public int? ActualQty { get; set; }

        [Required]
        public int Qty { get; set; }

        public string? Destination { get; set; }

        public string? ContNo { get; set; }
       
    }
}