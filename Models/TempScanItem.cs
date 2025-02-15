using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ScanBarcode.Models;

public partial class TempScanItem
{
    [Key]
    public int TempScanId { get; set; }
    [MaxLength(25)]
    public string SerialNumber { get; set; } = null!;

    public DateTime ScanTime { get; set; }

    public int PlanNumber { get; set; }

    public int ActualNumber { get; set; }

    public int ModelId { get; set; }


    public string LineProd { get; set; }
}
