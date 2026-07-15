
namespace CarAllowedApi.Data;

public class PublicHoliday
{
    public int Id { get; set; }
    public DateOnly HolidayDate { get; set; }
    public string HolidayName { get; set; } = "";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
}