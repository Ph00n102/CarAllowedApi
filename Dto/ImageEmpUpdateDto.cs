using System;

namespace CarAllowedApi.Dto;

public class ImageEmpUpdateDto
{
    public int Id { get; set; }
    // public int OpNoteId { get; set; }
    public string Name { get; set; }
    public string Nickname { get; set; }
    public string Tel { get; set; }
     public string? EmpStatusId { get; set; } //สถานะ
    public IFormFile? ImageFile { get; set; }
}
