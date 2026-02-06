// ในโฟลเดอร์ Dto
using System.ComponentModel.DataAnnotations;

namespace CarAllowedApi.Dto
{
    // DTO สำหรับสร้างเลขรัน
    public class JobNumberDto
    {
        [Required]
        public int JobId { get; set; }

        public string? JobNumber { get; set; }  // ถ้าไม่ส่งมาให้ระบบสร้างให้
        public bool ResetCounter { get; set; } = false; // ต้องการรีเซ็ตตัวนับหรือไม่
    }

    // DTO สำหรับอัพเดท Job Number
    public class JobNumberUpdateDto
    {
        [Required]
        public string JobNumber { get; set; } = string.Empty;
    }

    // DTO สำหรับผลลัพธ์
    public class JobNumberResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string JobNumber { get; set; } = string.Empty;
        public int JobId { get; set; }
        public DateTime GeneratedAt { get; set; }
        public int NextCounter { get; set; }
    }
    public class JobDistributeResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string JobNumber { get; set; } = string.Empty;
        public JobRequestCarDto Data { get; set; } = new JobRequestCarDto();
        public DateTime DistributedAt { get; set; }
    }


    public class ApiResponse<T>
    {
        public string? Message { get; set; }
        public T? Data { get; set; }
        public bool Success { get; set; }
    }

    // DTO สำหรับการจ่ายงานแบบเต็ม
    public class JobDistributeCarFullDto
    {
        public int? ImageEmpId { get; set; }
        public int? GarageId { get; set; }
        public DateOnly? JDDate { get; set; }
        public TimeOnly? JDTime { get; set; }
        public string? JobNumber { get; set; }
        public int JobStatusId { get; set; } = 2;
    }
    
}