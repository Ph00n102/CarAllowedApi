using System;

namespace CarAllowedApi.Dto;

public class JobImageDto
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public byte[]? ImageFile { get; set; }
    // public int JobRequestCarId { get; set; }
}
