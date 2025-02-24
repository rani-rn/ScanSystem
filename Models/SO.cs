using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace ScanBarcode.Models
{
 public class SOList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SOId { get; set; }

        public string SONumber { get; set; }

        public string Destination { get; set; }

        public DateTime Date { get; set; }

        public List<DeliveryOrder> DeliveryOrders { get; set; }
    }
}