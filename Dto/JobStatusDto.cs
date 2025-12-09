using System;
using CarAllowedApi.Data;

namespace CarAllowedApi.Dto;

public class JobStatusDto
{
    public int Id { get; set; } // หรืออาจเป็น int
    public string Status { get; set; }
    public string? Description { get; set; }
    
}
