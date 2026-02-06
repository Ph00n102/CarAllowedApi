using System.Text.Json.Serialization;

namespace CarAllowedApi.Data;

public class ImageEmp
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Nickname { get; set; }
    public string Empposition { get; set; }
    public string Tel { get; set; }
    public string FileName { get; set; }
    public byte[] ImageFile { get; set; }
    public string? EmpStatusId { get; set; } //สถานะ
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }

    [JsonIgnore] 
    public ICollection<JobRequestCar> JobRequestCars { get; set; }
}