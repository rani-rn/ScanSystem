using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ScanBarcode.Models;

public partial class Shipment
{
    [Key]
    public int ShipmentId { get; set; }

    public int Doid { get; set; }

    public DateTime ShipmentDate { get; set; }

    public string? ContNo { get; set; }

    public string Destination { get; set; } = null!;

    public int ModelId { get; set; }

    public int Qty { get; set; }

}
