namespace CarAllowedApi.Dto;


public class CreatePublicHolidayDto
{
    public DateOnly HolidayDate { get; set; }
    public string HolidayName { get; set; } = "";
    public bool IsActive { get; set; } = true;
}