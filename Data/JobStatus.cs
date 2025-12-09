using System;
using System.Text.Json.Serialization;

namespace CarAllowedApi.Data;

public class JobStatus
{
    public int Id { get; set; } // หรืออาจเป็น int
    public string Status { get; set; }
    public string? Description { get; set; }

    // Navigation property
    [JsonIgnore] 
    public ICollection<JobRequestCar> JobRequestCars { get; set; }
}
