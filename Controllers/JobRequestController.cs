using CarAllowedApi.Data;
using CarAllowedApi.Dto;
using CarAllowedApi.Hubs;
using CarAllowedApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using static CarAllowedApi.Services.JobRequestCarService;

namespace CarAllowedApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobRequestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        // private readonly IHosService _hosService;
        // private readonly IOpNotePdfService _opnoteService;
        private readonly IJobRequestCarService _service;
        private readonly IHubContext<JobRequestHub> _hubContext;

        public JobRequestController(ApplicationDbContext context, IJobRequestCarService service, IHubContext<JobRequestHub> hubContext)
        {
            _context = context;
            _service = service;
            _hubContext = hubContext;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<JobRequestCar[]>> GetAllJobRequestCars()
        {
            return Ok(await _service.GetAllJobRequestCarsAsync());
        }
        [HttpGet("GetAll1")]
        public async Task<ActionResult<IEnumerable<JobRequestCar>>> GetAll1JobRequestCars()
        {
            var list = await _service.GetAll1JobRequestCarsAsync();
            return Ok(list);
        }
        [HttpGet("GetAllSearch")]
        public async Task<ActionResult<IEnumerable<JobRequestCar>>> GetAllSearchJobRequestCars()
        {
            var list = await _service.GetAllSearchJobRequestCarsAsync();
            return Ok(list);
        }
        [HttpGet("GetAllDay")]
        public async Task<ActionResult<IEnumerable<JobRequestCar>>> GetAllDayJobRequestCars()
        {
            var list = await _service.GetAllDayJobRequestCarsAsync();
            return Ok(list);
        }
        [HttpGet("GetAllDay1")]
        public async Task<ActionResult<JobRequestCarAllDayDto[]>> GetAllDay1JobRequestCars()
        {
            try
            {
                var list = await _service.GetAllDay1JobRequestCarsAsync();
                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("GetAllEmpDay/{name?}")]
        public async Task<ActionResult<JobRequestCarAllDayDto[]>> GetAllEmpDayJobRequestCars(string name = null, int statusId = 0)
        {
            try
            {
                var list = await _service.GetAllEmpDayJobRequestCarsAsync(name,  statusId);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        [HttpGet("GetAllEmptoDay/{name?}")]
        public async Task<ActionResult<JobRequestCarAllDayDto[]>> GetAllEmpToDayJobRequestCars(string name = null)
        {
            try
            {
                var list = await _service.GetAllEmptoDayJobRequestCarsAsync(name);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("GetAllListDay/{name?}")]
        public async Task<ActionResult<JobRequestCarAllDayDto[]>> GetAllListDayJobRequestCars(string name = null, int statusId = 0)
        {
            try
            {
                var list = await _service.GetAllListDayJobRequestCarsAsync(name,  statusId);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetAllListtoDay/{name?}")]
        public async Task<ActionResult<JobRequestCarAllDayDto[]>> GetAllListtoDayJobRequestCars(string name = null)
        {
            try
            {
                var list = await _service.GetAllListtoDayJobRequestCarsAsync(name);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<JobRequestCar>> GetJobRequestCar(int id)
        {
            var result = await _service.GetJobRequestCarByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }
        [HttpGet("GetJobRequestCarsByStatus/{id}")]
        public async Task<ActionResult<JobRequestCar>> GetJobRequestCarsByStatus(int id)
        {
            var result = await _service.GetJobRequestCarsByStatusAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet("GetJobRequestNoImbyId/{id}")]
        public async Task<ActionResult<JobRequestCarNoImDto>> GetJobRequestNoImbyId(int id)
        {
            var result = await _service.GetJobRequestNoImByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        // GET: api/File/{id} - ดาวน์โหลดไฟล์โดย ID
        [HttpGet("DownloadFile/{id}")]
        public async Task<IActionResult> DownloadFile(int id)
        {
            try
            {
                Console.WriteLine($"DownloadFile requested for ID: {id}");

                // ค้นหาไฟล์จาก JobImage โดย ID
                var jobImage = await _context.JobImages
                    .AsNoTracking()
                    .FirstOrDefaultAsync(f => f.Id == id);

                if (jobImage == null)
                {
                    Console.WriteLine($"File not found with ID: {id}");
                    return NotFound(new
                    {
                        success = false,
                        message = "ไม่พบไฟล์ที่ต้องการ"
                    });
                }

                Console.WriteLine($"Found file: {jobImage.FileName}, Size: {jobImage.ImageFile?.Length ?? 0} bytes");

                // ตรวจสอบว่ามีข้อมูลไฟล์หรือไม่
                if (jobImage.ImageFile == null || jobImage.ImageFile.Length == 0)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "ไฟล์ว่างเปล่า"
                    });
                }

                // กำหนด Content-Type จากนามสกุลไฟล์
                var contentType = GetContentType(jobImage.FileName);

                // Return ไฟล์
                return File(jobImage.ImageFile, contentType, jobImage.FileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading file: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                return StatusCode(500, new
                {
                    success = false,
                    message = "เกิดข้อผิดพลาดในการดาวน์โหลดไฟล์",
                    error = ex.Message
                });
            }
        }

        // GET: api/File/info/{id} - ดึงข้อมูลไฟล์ (ไม่รวมเนื้อหาไฟล์)
        [HttpGet("info/{id}")]
        public async Task<IActionResult> GetFileInfo(int id)
        {
            try
            {
                var jobImage = await _context.JobImages
                    .AsNoTracking()
                    .FirstOrDefaultAsync(f => f.Id == id);

                if (jobImage == null)
                    return NotFound(new { success = false, message = "ไม่พบไฟล์" });

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        id = jobImage.Id,
                        fileName = jobImage.FileName,
                        fileSize = jobImage.ImageFile?.Length ?? 0,
                        contentType = GetContentType(jobImage.FileName),
                        opNoteDetailId = jobImage.OpNoteDetailId,
                        createdAt = DateTime.Now // หรือเพิ่มวันที่สร้างใน Model
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "เกิดข้อผิดพลาด",
                    error = ex.Message
                });
            }
        }

        // GET: api/File/byjob/{jobId} - ดึงไฟล์ทั้งหมดตาม Job ID
        [HttpGet("byjob/{jobId}")]
        public async Task<IActionResult> GetFilesByJobId(int jobId)
        {
            try
            {
                var files = await _context.JobImages
                    .Where(f => f.OpNoteDetailId == jobId)
                    .AsNoTracking()
                    .Select(f => new
                    {
                        id = f.Id,
                        fileName = f.FileName,
                        fileSize = f.ImageFile.Length,
                        contentType = GetContentType(f.FileName),
                        opNoteDetailId = f.OpNoteDetailId
                    })
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    count = files.Count,
                    data = files
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "เกิดข้อผิดพลาด",
                    error = ex.Message
                });
            }
        }

        // Helper method เพื่อกำหนด Content-Type
        private string GetContentType(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return "application/octet-stream";

            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            // Dictionary ของ Content-Type
            var contentTypes = new Dictionary<string, string>
            {
                { ".pdf", "application/pdf" },
                { ".jpg", "image/jpeg" },
                { ".jpeg", "image/jpeg" },
                { ".png", "image/png" },
                { ".gif", "image/gif" },
                { ".bmp", "image/bmp" },
                { ".txt", "text/plain" },
                { ".doc", "application/msword" },
                { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
                { ".xls", "application/vnd.ms-excel" },
                { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
                { ".zip", "application/zip" },
                { ".rar", "application/x-rar-compressed" },
                { ".mp4", "video/mp4" },
                { ".mp3", "audio/mpeg" },
                { ".wav", "audio/wav" },
                { ".avi", "video/x-msvideo" },
                { ".mov", "video/quicktime" },
                { ".csv", "text/csv" },
                { ".json", "application/json" },
                { ".xml", "application/xml" },
                { ".html", "text/html" },
                { ".htm", "text/html" },
                { ".css", "text/css" },
                { ".js", "application/javascript" }
            };

            return contentTypes.ContainsKey(extension)
                ? contentTypes[extension]
                : "application/octet-stream";
        }
        // [HttpPost("AddJobRequestCar")]
        // public async Task<IActionResult> CreateJobRequestCar([FromForm] JobRequestCarDto dto)
        // {
        //     try
        //     {
        //         // ตรวจสอบข้อมูลที่จำเป็น
        //         if (string.IsNullOrEmpty(dto.Requester))
        //             return BadRequest("Requester is required");
        //         if (string.IsNullOrEmpty(dto.DepartmentId))
        //             return BadRequest("DepartmentId is required");
        //         if (string.IsNullOrEmpty(dto.Origin))
        //             return BadRequest("Origin is required");
        //         if (string.IsNullOrEmpty(dto.Destination))
        //             return BadRequest("Destination is required");
        //         if (string.IsNullOrEmpty(dto.Tel))
        //             return BadRequest("Tel is required");

        //         var result = await _service.CreateJobRequestCarAsync(dto);
        //         return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        //     }
        //     catch (ArgumentException ex)
        //     {
        //         return BadRequest(ex.Message);
        //     }
        //     catch (InvalidOperationException ex)
        //     {
        //         return BadRequest(ex.Message);
        //     }
        //     catch (Exception ex)
        //     {
        //         return StatusCode(500, $"Internal server error: {ex.Message}");
        //     }
        // }
        [HttpPost("AddJobRequestCar")]
        public async Task<IActionResult> CreateJobRequestCar([FromForm] JobRequestCarDto dto)
        {
            try
            {
                // ตรวจสอบข้อมูลที่จำเป็น
                if (string.IsNullOrEmpty(dto.Requester))
                    return BadRequest("Requester is required");
                // ... validation อื่นๆ ...

                var result = await _service.CreateJobRequestCarAsync(dto);

                // ส่งข้อมูลใหม่ผ่าน SignalR
                if (result != null)
                {
                    // สร้างข้อมูลที่จะส่งกลับ (รวมถึง relations ด้วย)
                    var responseData = new
                    {
                        Id = result.Id,
                        DateNow = result.DateNow,
                        TimeNow = result.TimeNow,
                        Requester = result.Requester,
                        DepartmentId = result.DepartmentId,
                        Position = result.Position,
                        District = result.District,
                        Province = result.Province,
                        Origin = result.Origin,
                        Destination = result.Destination,
                        StartDate = result.StartDate,
                        StartTime = result.StartTime,
                        EndDate = result.EndDate,
                        EndTime = result.EndTime,
                        JobStatusId = result.JobStatusId,
                        JobStatus = result.JobStatus,
                        Garage = result.Garage,
                        ImageEmp = result.ImageEmp,
                        PerApplicant = result.PerApplicant,
                        PerPosition = result.PerPosition,
                        CreatedAt = DateTime.Now
                    };

                    // ส่งผ่าน SignalR
                    await _hubContext.Clients.All.SendAsync("ReceiveNewJobRequest", responseData);
                }

                return CreatedAtAction(nameof(GetById), new { id = result.Id }, new
                {
                    message = "Job request created successfully",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("UpdateJobRequestCar/{id}")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UpdateJobRequestCar(int id, [FromForm] JobRequestCarUpDto dto)
        {
            try
            {
                // ตรวจสอบข้อมูลที่จำเป็น (โค้ดเดิมของคุณ)
                if (string.IsNullOrEmpty(dto.Requester))
                    return BadRequest("Requester is required");
                // ... validation อื่นๆ ...

                // ตรวจสอบว่า id ใน URL ตรงกับข้อมูลที่จะอัพเดท
                var result = await _service.UpdateJobRequestCarAsync(id, dto);

                if (result == null)
                    return NotFound($"JobRequestCar with ID {id} not found");

                // ส่งข้อมูลอัพเดทผ่าน SignalR
                if (result != null)
                {
                    await _hubContext.Clients.All.SendAsync("ReceiveJobRequestUpdated", result);

                    // ส่ง notification
                    await _hubContext.Clients.All.SendAsync("ReceiveNotification",
                        new
                        {
                            Type = "info",
                            Message = $"รายการ {id} ถูกอัพเดทแล้ว",
                            Data = new { Id = result.Id, Requester = result.Requester }
                        });
                }

                return Ok(new
                {
                    message = "Update successful",
                    data = result
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // [HttpPut("UpdateJobRequestCar/{id}")]
        // public async Task<ActionResult<JobRequestCar>> UpdateJo0000000000000bRequestCar(int id, [FromBody] JobRequestCarDto dto)
        // {
        //     if (dto == null)
        //         return BadRequest("Input data is null");

        //     try
        //     {
        //         var updatedJobRequestCar = await _service.UpdateJobRequestCarAsync(id, dto);

        //         if (updatedJobRequestCar == null)
        //         {
        //             return NotFound($"Job request car with ID '{id}' not found");
        //         }

        //         return Ok(updatedJobRequestCar);
        //     }
        //     catch (ArgumentNullException ex)
        //     {
        //         return BadRequest(ex.Message);
        //     }
        //     catch (ArgumentException ex)
        //     {
        //         return BadRequest(ex.Message);
        //     }
        //     catch (Exception ex)
        //     {
        //         return StatusCode(500, $"Internal server error: {ex.Message}");
        //     }
        // }

        [HttpPut("UpdatePartial/{id}")]
        public async Task<ActionResult<JobRequestCar>> UpdateJobRequestCarPartial(
            int id,
            [FromBody] JobRequestCarPartialUpdateDto dto)
        {
            if (dto == null)
                return BadRequest("Input data is null");

            try
            {
                var updatedJobRequestCar = await _service.UpdateJobRequestCarPartialAsync(id, dto);

                if (updatedJobRequestCar == null)
                {
                    return NotFound($"Job request car with ID '{id}' not found");
                }

                // ตรวจสอบว่ามีการเปลี่ยนสถานะเป็น "ยกเลิก" หรือไม่
                if (dto.JobStatusId == 4) // สมมติว่า 4 คือสถานะ "ยกเลิก"
                {
                    // ส่งข้อมูลการยกเลิกผ่าน SignalR
                    await _hubContext.Clients.All.SendAsync("ReceiveJobRequestCancelled", id);

                    // ส่ง notification
                    await _hubContext.Clients.All.SendAsync("ReceiveNotification",
                        new
                        {
                            Type = "warning",
                            Message = $"รายการ {id} ถูกยกเลิกแล้ว",
                            Data = new { Id = id }
                        });
                }
                else
                {
                    // ส่งข้อมูลอัพเดทผ่าน SignalR
                    await _hubContext.Clients.All.SendAsync("ReceiveJobRequestUpdated", updatedJobRequestCar);
                }

                return Ok(updatedJobRequestCar);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("UpdateJobDistribute/{id}")]
        public async Task<ActionResult<JobRequestCar>> UpdateJobDistribute(
            int id,
            [FromBody] JobDistributeCarPartialDto dto)
        {
            if (dto == null)
                return BadRequest("Input data is null");

            try
            {
                var updatedJobRequestCar = await _service.UpdateJobDistributeCarPartialAsync(id, dto);

                if (updatedJobRequestCar == null)
                {
                    return NotFound($"Job request car with ID '{id}' not found");
                }

                return Ok(updatedJobRequestCar);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("UpdateJobAccepting/{id}")]
        public async Task<ActionResult<JobRequestCar>> UpdateJobAccepting(int id, [FromBody] JobAcceptingDto dto)
        {
            if (dto == null)
                return BadRequest("Input data is null");

            try
            {
                var updatedJobRequestCar = await _service.UpdateJobAcceptingAsync(id, dto);

                if (updatedJobRequestCar == null)
                {
                    return NotFound($"Job request car with ID '{id}' not found");
                }

                return Ok(updatedJobRequestCar);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // [HttpPost("PostJobRequestCar")]
        // public async Task<ActionResult<JobRequestCar>> PostJobRequestCar([FromBody] JobRequestCarDto dto)
        // {
        //     if (dto == null)
        //         return BadRequest("Input data is null");

        //     try
        //     {
        //         var jobRequestCar = new JobRequestCar
        //         {
        //             DateNow = dto.DateNow,
        //             TimeNow = dto.TimeNow,
        //             EDateNow = dto.EDateNow,
        //             ETimeNow = dto.ETimeNow,
        //             Requester = dto.Requester ?? string.Empty,
        //             DepartmentId = dto.DepartmentId ?? string.Empty,
        //             Origin = dto.Origin ?? string.Empty,
        //             Destination = dto.Destination ?? string.Empty,
        //             StartDate = dto.StartDate,
        //             StartTime = dto.StartTime,
        //             EndDate = dto.EndDate,
        //             EndTime = dto.EndTime,
        //             NumPer = dto.NumPer ?? 0,
        //             Tel = dto.Tel ?? string.Empty,
        //             Note = dto.Note ?? string.Empty,
        //             ImageFiles = dto.ImageFiles?.Select(img => new JobImage
        //             {
        //                 FileName = img.FileName ?? string.Empty,
        //                 ImageFile = img.ImageFile
        //             }).ToList() ?? new List<JobImage>()
        //         };

        //         _context.JobRequestCars.Add(jobRequestCar);
        //         await _context.SaveChangesAsync();

        //         return CreatedAtAction(nameof(GetJobRequestCar), new { id = jobRequestCar.Id }, jobRequestCar);
        //     }
        //     catch (Exception ex)
        //     {
        //         // Log error here if you have logging
        //         return StatusCode(500, $"Internal server error: {ex.Message}");
        //     }
        // }
        // [HttpGet("{id}")]
        // public async Task<ActionResult<JobRequestCar>> GetJobRequestCar(int id)
        // {
        //     var jobRequestCar = await _context.JobRequestCars
        //         .Include(j => j.ImageFiles)
        //         .FirstOrDefaultAsync(j => j.Id == id);

        //     if (jobRequestCar == null)
        //     {
        //         return NotFound();
        //     }

        //     return jobRequestCar;
        // }
        [HttpGet("StatusGet")]
        public async Task<IActionResult> GetAll() =>
            Ok(await _service.GetAllJobStatusesAsync());

        [HttpGet("Status/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetJobStatusByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost("StatusAdd")]
        public async Task<IActionResult> StatusCreate([FromBody] JobStatusDto dto)
        {
            try
            {
                var result = await _service.CreateJobStatusAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("StatusUpdate/{id}")]
        public async Task<IActionResult> StatusUpdate(int id, [FromBody] JobStatusDto dto)
        {
            try
            {
                var result = await _service.UpdateJobStatusAsync(id, dto);
                return result == null ? NotFound() : Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("StatusDelete/{id}")]
        public async Task<IActionResult> StatusDelete(int id)
        {
            try
            {
                var result = await _service.DeleteJobStatusAsync(id);
                return result ? NoContent() : NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("DistributeJob/{id}")]
        public async Task<ActionResult<JobRequestCar>> DistributeJob(
            int id,
            [FromBody] JobDistributeCarFullDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Input data is null");

                // ตรวจสอบข้อมูลที่จำเป็น
                if (!dto.ImageEmpId.HasValue)
                    return BadRequest("ต้องระบุคนขับ");
                if (!dto.GarageId.HasValue)
                    return BadRequest("ต้องเลือกรถ");

                // เรียกใช้ service สำหรับการจ่ายงานแบบมีเลขรัน
                var updatedJob = await _service.DistributeJobWithNumberAsync(id, dto);

                if (updatedJob == null)
                    return NotFound($"Job request car with ID '{id}' not found");

                // ส่งข้อมูลผ่าน SignalR
                await _hubContext.Clients.All.SendAsync("ReceiveJobDistributed", new
                {
                    Id = updatedJob.Id,
                    JobNumber = updatedJob.JobNumber,
                    ImageEmpId = updatedJob.ImageEmpId,
                    GarageId = updatedJob.GarageId,
                    JDDate = updatedJob.JDDate,
                    JDTime = updatedJob.JDTime,
                    Status = updatedJob.JobStatus?.Status,
                    DistributedAt = DateTime.Now
                });

                // ส่ง notification
                await _hubContext.Clients.All.SendAsync("ReceiveNotification",
                    new
                    {
                        Type = "success",
                        Message = $"จ่ายงาน #{updatedJob.JobNumber} สำเร็จ",
                        Data = new
                        {
                            Id = updatedJob.Id,
                            JobNumber = updatedJob.JobNumber,
                            Requester = updatedJob.Requester
                        }
                    });

                return Ok(new
                {
                    message = "จ่ายงานสำเร็จ",
                    jobNumber = updatedJob.JobNumber,
                    data = updatedJob
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("UpdateJobNumber/{id}")]
        public async Task<ActionResult> UpdateJobNumber(int id, [FromBody] JobNumberUpdateDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.JobNumber))
                    return BadRequest("Job Number ไม่สามารถเป็นค่าว่างได้");

                var updatedJob = await _service.UpdateJobNumberAsync(id, dto.JobNumber);

                if (updatedJob == null)
                    return NotFound($"Job request car with ID '{id}' not found");

                // ส่งข้อมูลผ่าน SignalR
                await _hubContext.Clients.All.SendAsync("ReceiveJobNumberUpdated", new
                {
                    JobId = id,
                    JobNumber = dto.JobNumber,
                    UpdatedAt = DateTime.Now
                });

                return Ok(new
                {
                    message = "อัพเดทเลขรันสำเร็จ",
                    data = updatedJob
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetJobNumberInfo/{jobId}")]
        public async Task<ActionResult> GetJobNumberInfo(int jobId)
        {
            try
            {
                var result = await _service.GetJobNumberInfoForJobAsync(jobId);

                if (!result.Success)
                    return BadRequest(result.Message);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
