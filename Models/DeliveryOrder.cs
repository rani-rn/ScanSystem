using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ScanBarcode.Models;

public partial class DeliveryOrder
{
    [Key]
    public int Doid { get; set; }

    public string Donumber { get; set; } = null!;

    public DateTime RequestedDate { get; set; }

    public int ModelId { get; set; }

    public int? Qty { get; set; }

    public string? Destination { get; set; }

}
