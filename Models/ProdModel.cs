using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ScanBarcode.Models;

public partial class ProdModel
{
    [Key]
    public int ModelId { get; set; }

    public string ModelName { get; set; } = null!;

    public int PkgSize { get; set; }

  
}
