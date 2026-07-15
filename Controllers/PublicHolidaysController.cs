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
    [ApiController]
    [Route("api/public-holidays")]
    public class PublicHolidaysController : ControllerBase
    {
        private readonly IPublicHolidayService _service;

        public PublicHolidaysController(IPublicHolidayService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? year = null)
        {
            var data = await _service.GetAllAsync(year);
            return Ok(data);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _service.GetByIdAsync(id);

            if (data == null)
                return NotFound("ไม่พบข้อมูลวันหยุด");

            return Ok(data);
        }

        [HttpGet("check/{date}")]
        public async Task<IActionResult> CheckHoliday(DateOnly date)
        {
            var isHoliday = await _service.IsHolidayAsync(date);

            return Ok(new
            {
                date,
                isHoliday
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePublicHolidayDto dto)
        {
            var result = await _service.CreateAsync(dto);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Data);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, CreatePublicHolidayDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Data);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);

            if (!success)
                return NotFound("ไม่พบข้อมูลวันหยุด");

            return Ok(new
            {
                message = "ลบข้อมูลสำเร็จ"
            });
        }
    }
        
}
