using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ScanBarcode.Models
{
    public class SO
    {
        [Key]
        public int SOId {get; set;}
        public string SONumber {get; set;}

        public string Destination {get; set;}
    }
}