using System;

namespace CarAllowedApi.Data;

public class ImageFile
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public byte[] FileData { get; set; }
    public string ContentType { get; set; }
    public long FileSize { get; set; }
    // Foreign key
    public int JobRequestCarId { get; set; }
    public JobRequestCar JobRequestCar { get; set; }
}
