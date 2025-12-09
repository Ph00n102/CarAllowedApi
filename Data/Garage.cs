namespace CarAllowedApi.Data;

public class Garage
{
    public int Id { get; set; }
    public string CarRegistration { get; set; }
    public string Carmodel { get; set; }
    public string Cartype { get; set; }
    public string? CarStatusId { get; set; } //สถานะ
    public string? CarProvince { get; set; } //จังหวัด
    public string FileName { get; set; }
    public byte[] ImageFile { get; set; }
     // Optional: เพิ่ม timestamp
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
}