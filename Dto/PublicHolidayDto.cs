namespace CarAllowedApi.Dto;

public class PublicHolidayDto
{
    public int Id { get; set; }
    public DateOnly HolidayDate { get; set; }
    public string HolidayName { get; set; } = "";
    public bool IsActive { get; set; }
}

 