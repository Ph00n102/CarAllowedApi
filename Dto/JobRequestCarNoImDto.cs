using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using CarAllowedApi.Data;

namespace CarAllowedApi.Dto;

public class JobRequestCarNoImDto
{
    public int Id { get; set; }
    public DateOnly DateNow { get; set; }
    public TimeOnly TimeNow { get; set; }
    public DateOnly EDateNow { get; set; }
    public TimeOnly ETimeNow { get; set; }
    public string? Requester { get; set; }
    public string? DepartmentId { get; set; }
    public string? Position { get; set; } //หน่วยงาน
    public string? District { get; set; } //หน่วยงาน
    public string? Province { get; set; } //หน่วยงาน
    public string? Origin { get; set; }
    public string? Destination { get; set; }
    public string? AlongWith { get; set; }
    public string? For { get; set; }
    public string? Location { get; set; }
    public TimeOnly LocationTime { get; set; }
    public DateOnly StartDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public DateOnly EndDate { get; set; }
    public TimeOnly EndTime { get; set; }
    public DateOnly JDDate { get; set; }  //วันที่ที่สิ้นสุดการทำงาน
    public TimeOnly JDTime { get; set; }  //เวลาที่สิ้นสุดการทำงาน
    public DateOnly StatusDate { get; set; }  //วันที่ที่สิ้นสุดการทำงาน
    public TimeOnly StatusTime { get; set; }  //เวลาที่สิ้นสุดการทำงาน
    public int JobStatusId { get; set; }

    // ใช้ string แทน object ถ้าต้องการแค่ชื่อ
    public string? JobStatusName { get; set; }

    public int? ImageEmpId { get; set; }

    // ใช้ string แทน object ถ้าต้องการแค่ชื่อคนขับ
    public string? ImageEmpName { get; set; }

    public int? GarageId { get; set; }
    public string? Cartype { get; set; }

    // ใช้ string แทน object ถ้าต้องการแค่ทะเบียนรถ
    public string? GarageCarRegistration { get; set; }

    public int? NumPer { get; set; }
    public string? Tel { get; set; }
    public string? Note { get; set; }
    public string? Ot { get; set; }
    public string? MileageOut { get; set; }
    public string? MileageBack { get; set; }
    public int? NumOil { get; set; } //จำนวนน้ำมัน
    public string? Price { get; set; }
    public DateOnly IssueDate { get; set; }  //วันที่ที่ออกทำงาน
    public TimeOnly IssueTime { get; set; }  //เวลาที่ออกทำงาน
    public DateOnly ReturnDate { get; set; }  //วันที่ที่กลับจากทำงาน
    public TimeOnly ReturnTime { get; set; }  //เวลาที่กลับจากทำงาน
    public DateOnly UpNDate { get; set; }  //วันที่ที่กลับจากทำงาน
    public TimeOnly UpNTime { get; set; }  //เวลาที่กลับจากทำงาน
    public string? PerApplicant { get; set; } //ผู้ขออนุญาต 
    public string? PerPosition { get; set; } //ตำแหน่งผู้ขอ
    public string? JobNumber { get; set; } 
    
}