using System.ComponentModel.DataAnnotations;

namespace CarAllowedApi.Dto;
public class JobRequestCarResponseDto
{
    public int Id { get; set; }
    public string DateNow { get; set; }
    public string TimeNow { get; set; }
    public string EDateNow { get; set; }
    public string ETimeNow { get; set; }
    public string Requester { get; set; }
    public string DepartmentId { get; set; }
    public string Origin { get; set; }
    public string Destination { get; set; }
    public string StartDate { get; set; }
    public string StartTime { get; set; }
    public string EndDate { get; set; }
    public string EndTime { get; set; }
    public int? JobStatusId { get; set; }
    public string JobStatusName { get; set; }
    public int? ImageEmpId { get; set; }
    public string ImageEmpName { get; set; }
    public int? GarageId { get; set; }
    public string GarageName { get; set; }
    public int? NumPer { get; set; }
    public string Tel { get; set; }
    public string Note { get; set; }
    public int ImageFileCount { get; set; }
    public List<string> ImageFilePaths { get; set; } = new();
}

// UpdateStatusDto สำหรับอัพเดทสถานะ
public class UpdateStatusDto
{
    [Required]
    public int JobStatusId { get; set; }
}