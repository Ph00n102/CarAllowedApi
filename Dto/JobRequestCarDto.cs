using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CarAllowedApi.Dto;

public class JobRequestCarDto
{
    // รับเป็น string จาก client แล้วแปลงใน service

    public string? DateNow { get; set; }
    public string? TimeNow { get; set; }
    public string? EDateNow { get; set; }
    public string? ETimeNow { get; set; }

    [Required(ErrorMessage = "Requester is required")]
    public string? Requester { get; set; } //ผู้ขอใช้รถ

    [Required(ErrorMessage = "DepartmentId is required")]
    public string? DepartmentId { get; set; } //หน่วยงาน

    [Required(ErrorMessage = "Position is required")]
    public string? Position { get; set; } //หน่วยงาน
    [Required(ErrorMessage = "District is required")]
    public string? District { get; set; } //หน่วยงาน
    [Required(ErrorMessage = "Province is required")]
    public string? Province { get; set; } //หน่วยงาน

    [Required(ErrorMessage = "Origin is required")]
    public string? Origin { get; set; } //ต้นทาง

    [Required(ErrorMessage = "Destination is required")]
    public string? Destination { get; set; } //ปลายทาง
    public string? AlongWith { get; set; } //พร้อมด้วย
    public string? For { get; set; } //เพื่อ
    public string? Location { get; set; } //สถาที่รับ
    public string? LocationTime { get; set; }  //เวลาไปรับ

    [Required(ErrorMessage = "StartDate is required")]
    public string? StartDate { get; set; }  //วันที่ที่เริ่มทำงาน

    [Required(ErrorMessage = "StartTime is required")]
    public string? StartTime { get; set; }  //เวลาที่เริ่มทำงาน

    [Required(ErrorMessage = "EndDate is required")]
    public string? EndDate { get; set; }  //วันที่ที่สิ้นสุดการทำงาน

    [Required(ErrorMessage = "EndTime is required")]
    public string? EndTime { get; set; }  //เวลาที่สิ้นสุดการทำงาน

    public int? JobStatusId { get; set; } //สถานะ
    public int? ImageEmpId { get; set; } //สถานะคนขับ
    public int? GarageId { get; set; } //สถานะรถ
    public int? NumPer { get; set; } //จำนวนคน

    [Required(ErrorMessage = "Tel is required")]
    [Phone(ErrorMessage = "Invalid phone number format")]
    public string? Tel { get; set; } //เบอร์ติดต่อ

    public string? Note { get; set; }
    public List<IFormFile>? ImageFiles { get; set; } 
    public string? PerApplicant { get; set; } //ผู้ขออนุญาต 
    public string? PerPosition { get; set; } //ตำแหน่งผู้ขอ
}