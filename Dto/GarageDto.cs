namespace CarAllowedApi.Dto;

public class GarageDto
{
    public int Id { get; set; }
    public string CarRegistration { get; set; }
    public string Carmodel { get; set; }
    public string Cartype { get; set; }
    public string? CarStatusId { get; set; }
    public string? CarProvince { get; set; } //จังหวัด
    public string FileName { get; set; }
    public IFormFile ImageFile { get; set; } 
     // Optional: เพิ่ม timestamp
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
}