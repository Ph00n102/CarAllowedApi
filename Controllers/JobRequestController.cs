using CarAllowedApi.Data;
using CarAllowedApi.Dto;
using CarAllowedApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        public JobRequestController(ApplicationDbContext context, IJobRequestCarService service)
        {
            _context = context;
            _service = service;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<JobRequestCar[]>> GetAllJobRequestCars()
        {
            return Ok(await _service.GetAllJobRequestCarsAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<JobRequestCar>> GetJobRequestCar(int id)
        {
            var result = await _service.GetJobRequestCarByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }
        [HttpPost("AddJobRequestCar")]
        public async Task<IActionResult> CreateJobRequestCar([FromForm] JobRequestCarDto dto)
        {
            try
            {
                // ตรวจสอบข้อมูลที่จำเป็น
                if (string.IsNullOrEmpty(dto.Requester))
                    return BadRequest("Requester is required");
                if (string.IsNullOrEmpty(dto.DepartmentId))
                    return BadRequest("DepartmentId is required");
                if (string.IsNullOrEmpty(dto.Origin))
                    return BadRequest("Origin is required");
                if (string.IsNullOrEmpty(dto.Destination))
                    return BadRequest("Destination is required");
                if (string.IsNullOrEmpty(dto.Tel))
                    return BadRequest("Tel is required");

                var result = await _service.CreateJobRequestCarAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
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
        [HttpPut("UpdateJobRequestCar/{id}")]
        public async Task<ActionResult<JobRequestCar>> UpdateJobRequestCar(int id, [FromBody] JobRequestCarDto dto)
        {
            if (dto == null)
                return BadRequest("Input data is null");

            try
            {
                var updatedJobRequestCar = await _service.UpdateJobRequestCarAsync(id, dto);
                
                if (updatedJobRequestCar == null)
                {
                    return NotFound($"Job request car with ID '{id}' not found");
                }

                return Ok(updatedJobRequestCar);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
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
    }    
}
