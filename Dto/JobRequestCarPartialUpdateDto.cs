using System.ComponentModel.DataAnnotations;

namespace CarAllowedApi.Dto
{
    public class JobRequestCarPartialUpdateDto
    {
        public DateOnly? StatusDate { get; set; }  //วันที่ที่สิ้นสุดการทำงาน
        public TimeOnly? StatusTime { get; set; }  //เวลาที่สิ้นสุดการทำงาน
        public int? JobStatusId { get; set; }
    }
}