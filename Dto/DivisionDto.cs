using System;
using System.Collections.Generic;

namespace CarAllowedApi.Dto;

public partial class DivisionDto
{
    public int Id { get; set; }

    public string DivisionName { get; set; } = null!;

    public string? ChiefName { get; set; }

    public string? Cid { get; set; }

    public string? LoginNameHosxp { get; set; }

    public string? LoginNameCustom { get; set; }
}
