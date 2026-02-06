
namespace CarAllowedApi.Dto;
public class SearchResultDto
{
    public int Id { get; set; }
    public string? Requester { get; set; }
    public string? DepartmentId { get; set; }
    public string? Position { get; set; }
    public string? Origin { get; set; }
    public string? Destination { get; set; }
    public string? Tel { get; set; }
    public string? District { get; set; }
    public string? Province { get; set; }
    public string? AlongWith { get; set; }
    public string? For { get; set; }
    public string? Location { get; set; }
    public string? PerApplicant { get; set; }
    public string? PerPosition { get; set; }
}