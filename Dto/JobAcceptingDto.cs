using System.ComponentModel.DataAnnotations;

namespace CarAllowedApi.Dto
{
    public class JobAcceptingDto
    {
        public string? Ot { get; set; } 
        public string? MileageOut { get; set; } 
        public string? MileageBack { get; set; } 
        public int? NumOil { get; set; } //จำนวนน้ำมัน
        public string? Price { get; set; } 
        public int? JobStatusId { get; set; }
        public DateOnly? IssueDate { get; set; }  //วันที่ที่ออกทำงาน
        public TimeOnly? IssueTime { get; set; }  //เวลาที่ออกทำงาน
        public DateOnly? ReturnDate { get; set; }  //วันที่ที่กลับจากทำงาน
        public TimeOnly? ReturnTime { get; set; }  //เวลาที่กลับจากทำงาน
        public DateOnly? UpNDate { get; set; }  //วันที่ที่กลับจากทำงาน
        public TimeOnly? UpNTime { get; set; }  //เวลาที่กลับจากทำงาน
    }
}