using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ScanBarcode.Models;

public partial class Rfidtag
{
    [Key]
    public int Id { get; set; }

    public string TagNumber { get; set; } 

    public DateTime CreateDateTime { get; set; }

    public string Model { get; set; } 

    public bool Register { get; set; }
}
