using CarAllowedApi.Data;
using CarAllowedApi.Dto;
using CarAllowedApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarAllowedApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageEmpController : ControllerBase
    {
        private readonly IImageEmpService _imageEmpService;
        private readonly ApplicationDbContext _context;

        public ImageEmpController(ApplicationDbContext context, IImageEmpService imageEmpService)
        {
            _context = context;
            _imageEmpService = imageEmpService;
        }

        [HttpGet]
        public async Task<ActionResult<ImageEmp[]>> GetImages()
        {
            try
            {
                var images = await _imageEmpService.GetImageAsync();
                return Ok(images);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ImageEmp>> GetImageById(int id)
        {
            try
            {
                var image = await _imageEmpService.GetImageByIdFirstAsync(id);
                if (image == null)
                {
                    return NotFound($"Image with ID {id} not found");
                }
                return Ok(image);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/ImageEmp
        [HttpPost]
        public async Task<ActionResult<ImageEmp>> SaveImage([FromForm] ImageEmpDto imageEmpDto)
        {
            try
            {
                if (imageEmpDto.ImageFile == null)
                    return BadRequest("Image file is required");

                // ตรวจสอบข้อมูลที่จำเป็น
                if (string.IsNullOrEmpty(imageEmpDto.Name))
                    return BadRequest("Name is required");
                if (string.IsNullOrEmpty(imageEmpDto.Nickname))
                    return BadRequest("Nickname is required");
                if (string.IsNullOrEmpty(imageEmpDto.Tel))
                    return BadRequest("Tel is required");
                if (string.IsNullOrEmpty(imageEmpDto.EmpStatusId))
                    return BadRequest("Tel is required");

                var result = await _imageEmpService.SaveImageAsync(imageEmpDto);
                return CreatedAtAction(nameof(GetImageById), new { id = result.Id }, result);
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

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromForm] ImageEmpUpdateDto update)
        {
            try
            {
                if (id != update.Id)
                    return BadRequest("ID mismatch");

                // ตรวจสอบว่ามีข้อมูลที่ต้องอัปเดตหรือไม่
                if (string.IsNullOrEmpty(update.Name) &&
                    string.IsNullOrEmpty(update.Nickname) &&
                    string.IsNullOrEmpty(update.Tel) &&
                    string.IsNullOrEmpty(update.EmpStatusId) &&
                    update.ImageFile == null)
                {
                    return BadRequest("No data to update");
                }

                var updated = await _imageEmpService.UpdateImageAsync(update);
                return Ok(updated);
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _imageEmpService.DeleteImageAsync(id);
                return NoContent();
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

        // เพิ่ม endpoint สำหรับค้นหา
        [HttpGet("search")]
        public async Task<ActionResult<ImageEmp[]>> SearchImages(
            [FromQuery] string? name = null,
            [FromQuery] string? nickname = null,
            [FromQuery] string? tel = null)
        {
            try
            {
                var query = _context.ImageEmps.AsQueryable();

                if (!string.IsNullOrEmpty(name))
                    query = query.Where(x => x.Name.Contains(name));

                if (!string.IsNullOrEmpty(nickname))
                    query = query.Where(x => x.Nickname.Contains(nickname));

                if (!string.IsNullOrEmpty(tel))
                    query = query.Where(x => x.Tel.Contains(tel));

                var results = await query
                    .OrderByDescending(x => x.Id)
                    .ToArrayAsync();

                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetCarImages")]
        public async Task<ActionResult<ImageGDto[]>> GetCarImages()
        {
            try
            {
                var images = await _imageEmpService.GetCarImageAsync();
                return Ok(images);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetCarImageById/{id}")]
        public async Task<ActionResult<Garage>> GetCarImageById(int id)
        {
            try
            {
                var image = await _imageEmpService.GetCarImageByIdFirstAsync(id);
                if (image == null)
                {
                    return NotFound($"Image with ID {id} not found");
                }
                return Ok(image);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost("SaveCarImage")]
        public async Task<ActionResult<Garage>> SaveCarImage([FromForm] GarageDto garageDto)
        {
            try
            {
                if (garageDto.ImageFile == null)
                    return BadRequest("Image file is required");

                // ตรวจสอบข้อมูลที่จำเป็น
                if (string.IsNullOrEmpty(garageDto.CarRegistration))
                    return BadRequest("Name is required");
                if (string.IsNullOrEmpty(garageDto.Carmodel))
                    return BadRequest("Nickname is required");
                if (string.IsNullOrEmpty(garageDto.Cartype))
                    return BadRequest("Tel is required");
                if (string.IsNullOrEmpty(garageDto.CarStatusId))
                    return BadRequest("CarStatusId is required");
                if (string.IsNullOrEmpty(garageDto.CarProvince))
                    return BadRequest("CarProvince is required");

                var result = await _imageEmpService.SaveCarImageAsync(garageDto);
                return CreatedAtAction(nameof(GetImageById), new { id = result.Id }, result);
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

        [HttpPut("CarUpdate/{id}")]
        public async Task<ActionResult> CarUpdate(int id, [FromForm] GarageDto update)
        {
            try
            {
                if (id != update.Id)
                    return BadRequest("ID mismatch");

                // ตรวจสอบว่ามีข้อมูลที่ต้องอัปเดตหรือไม่
                if (string.IsNullOrEmpty(update.CarRegistration) &&
                    string.IsNullOrEmpty(update.Carmodel) &&
                    string.IsNullOrEmpty(update.Cartype) &&
                    string.IsNullOrEmpty(update.CarStatusId) &&
                    string.IsNullOrEmpty(update.CarProvince) &&
                    update.ImageFile == null)
                {
                    return BadRequest("No data to update");
                }

                var updated = await _imageEmpService.UpdateCarImageAsync(update);
                return Ok(updated);
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
        [HttpDelete("CarDelete/{id}")]
        public async Task<IActionResult> CarDelete(int id)
        {
            try
            {
                await _imageEmpService.DeleteCarImageAsync(id);
                return NoContent();
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
    }
}