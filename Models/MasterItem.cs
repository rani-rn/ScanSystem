using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ScanBarcode.Models;

public partial class MasterItem
{
    [Key]
    public int ItemId { get; set; }

    [MaxLength(25)]
    public string SerialNumber { get; set; } = null!;

    public int? RfidtagId { get; set; }

    public DateTime AddedDateTime { get; set; }

    public string Model { get; set; }

    public string LineProduction { get; set; }

    
}
