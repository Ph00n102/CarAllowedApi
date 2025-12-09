using System.ComponentModel.DataAnnotations;

namespace CarAllowedApi.Dto
{
    public class JobRequestCarPartialUpdateDto
    {
        public DateOnly? EDateNow { get; set; }
        public TimeOnly? ETimeNow { get; set; }
        public int? JobStatusId { get; set; }
    }
}