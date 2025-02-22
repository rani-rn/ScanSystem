using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ScanBarcode.Models;

public partial class MasterTable
{
    [Key]
    public int MasterId { get; set; }
    public string SerialNumber { get; set; } = null!;
    public string Model { get; set; }
    public string Donumber { get; set; }
    public string LineProduction { get; set; }
    public string Destination { get; set; }
    public string? ContNo { get; set; }
    public int? ShipmentId { get; set; }
    public DateTime? ShipmentDateTime { get; set; }
}
