using CarAllowedApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarAllowedApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobNumberController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public JobNumberController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/JobNumber/GetLastJobNumber
        [HttpGet("GetLastJobNumber")]
        public async Task<IActionResult> GetLastJobNumber()
        {
            try
            {
                var lastJobNumber = await _context.JobRequestCars
                    .Where(j => !string.IsNullOrEmpty(j.JobNumber))
                    .OrderByDescending(j => j.JobNumber)
                    .Select(j => j.JobNumber)
                    .FirstOrDefaultAsync();
                    
                return Ok(lastJobNumber ?? "000000");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"เกิดข้อผิดพลาด: {ex.Message}");
            }
        }

        // GET: api/JobNumber/GetMaxJobNumber
        [HttpGet("GetMaxJobNumber")]
        public async Task<IActionResult> GetMaxJobNumber()
        {
            try
            {
                var maxJobNumber = await _context.JobRequestCars
                    .Where(j => !string.IsNullOrEmpty(j.JobNumber))
                    .Select(j => j.JobNumber)
                    .ToListAsync();

                if (!maxJobNumber.Any())
                    return Ok("000000");

                int maxNumber = 0;
                foreach (var jobNum in maxJobNumber)
                {
                    if (int.TryParse(jobNum, out int num) && num > maxNumber)
                        maxNumber = num;
                }

                return Ok(maxNumber.ToString("D6"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"เกิดข้อผิดพลาด: {ex.Message}");
            }
        }

        // PUT: api/JobNumber/UpdateJobNumber/{id}
        [HttpPut("UpdateJobNumber/{id}")]
        public async Task<IActionResult> UpdateJobNumber(int id, [FromBody] UpdateJobNumberDto dto)
        {
            try
            {
                var jobRequestCar = await _context.JobRequestCars.FindAsync(id);
                if (jobRequestCar == null)
                    return NotFound();

                jobRequestCar.JobNumber = dto.JobNumber;
                await _context.SaveChangesAsync();
                
                return Ok(new { message = "อัพเดทเลขรันเรียบร้อยแล้ว", jobNumber = dto.JobNumber });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"เกิดข้อผิดพลาด: {ex.Message}");
            }
        }

        // GET: api/JobNumber/GetJobNumber/{id}
        [HttpGet("GetJobNumber/{id}")]
        public async Task<IActionResult> GetJobNumber(int id)
        {
            try
            {
                var jobNumber = await _context.JobRequestCars
                    .Where(j => j.Id == id)
                    .Select(j => j.JobNumber)
                    .FirstOrDefaultAsync();
                    
                return Ok(jobNumber ?? string.Empty);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"เกิดข้อผิดพลาด: {ex.Message}");
            }
        }
    }

    public class UpdateJobNumberDto
    {
        public string JobNumber { get; set; } = string.Empty;
    }
}