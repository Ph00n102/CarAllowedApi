

using CarAllowedApi.Dto;
using CarAllowedApi.Hubs;
using CarAllowedApi.Models1;
using CarAllowedApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace CarAllowedApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DivisionsController : ControllerBase
    {
        private readonly DivisionsDbContext _context;
        // private readonly IHosService _hosService;
        // private readonly IOpNotePdfService _opnoteService;
        private readonly IDivisionsService _service;
        private readonly IHubContext<JobRequestHub> _hubContext;

        public DivisionsController(DivisionsDbContext context, IDivisionsService service, IHubContext<JobRequestHub> hubContext)
        {
            _context = context;
            _service = service;
            _hubContext = hubContext;
        }

        [HttpGet("GetDivision/{name?}")]
        public async Task<ActionResult<DivisionDto[]>> GetDivision(string name = null)
        {
            try
            {
                var list = await _service.GetDivisionAsync(name);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
    }
}
