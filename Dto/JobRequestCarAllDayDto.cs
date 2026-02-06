using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using CarAllowedApi.Data;

namespace CarAllowedApi.Dto;

public class JobRequestCarAllDayDto
{
    // รับเป็น string จาก client แล้วแปลงใน service
    public int Id { get; set; }
    public DateOnly DateNow { get; set; } = DateOnly.FromDateTime(DateTime.Now); //วันที่ที่ทำใบ
    public TimeOnly TimeNow { get; set; } = TimeOnly.FromDateTime(DateTime.Now); //เวลาที่ทำใบ
    public DateOnly EDateNow { get; set; } = DateOnly.FromDateTime(DateTime.Now); //วันที่ที่ยืนยันเสร็จงาน
    public TimeOnly ETimeNow { get; set; } = TimeOnly.FromDateTime(DateTime.Now); //เวลาที่ยืนยันเสร็จงาน
    public string? Requester { get; set; } //ผู้ขอใช้รถ
    public string? DepartmentId { get; set; } //หน่วยงาน
    public string? Origin { get; set; } //ต้นทาง
    public string? Destination { get; set; } //ปลายทาง
    public DateOnly StartDate { get; set; }  //วันที่ที่เริ่มทำงาน
    public TimeOnly StartTime { get; set; }  //เวลาที่เริ่มทำงาน
    public DateOnly EndDate { get; set; }  //วันที่ที่สิ้นสุดการทำงาน
    public TimeOnly EndTime { get; set; }  //เวลาที่สิ้นสุดการทำงาน
    public int JobStatusId { get; set; } //สถานะงาน
    public virtual JobStatus? JobStatus { get; set; }
    public int? ImageEmpId { get; set; } //สถานะคนขับ
    public virtual ImageEmp? ImageEmp { get; set; }
    public int? GarageId { get; set; } //สถานะรถ
    public virtual Garage? Garage { get; set; }
    public int? NumPer { get; set; } //จำนวนคน
    public string? Tel { get; set; } //เบอร์ติดต่อ
    public string? Note { get; set; } 
    public string? Ot { get; set; } 
    public string? MileageOut { get; set; } 
    public string? MileageBack { get; set; } 
    public int? NumOil { get; set; } //จำนวนน้ำมัน
    public string? Price { get; set; } 
    public string? JobNumber { get; set; } 
    public DateOnly IssueDate { get; set; }  //วันที่ที่ออกทำงาน
    public TimeOnly IssueTime { get; set; }  //เวลาที่ออกทำงาน
    public DateOnly ReturnDate { get; set; }  //วันที่ที่กลับจากทำงาน
    public TimeOnly ReturnTime { get; set; }  //เวลาที่กลับจากทำงาน
}