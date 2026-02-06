using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CarAllowedApi.Dto;

public class JobDistributeCarPartialDto
{
    public DateOnly? JDDate { get; set; }  //วันที่ที่สิ้นสุดการทำงาน
    public TimeOnly? JDTime { get; set; }  //เวลาที่สิ้นสุดการทำงาน
    public int? ImageEmpId { get; set; } //สถานะคนขับ
    public int? GarageId { get; set; } //สถานะรถ
    public string? JobNumber { get; set; }
}