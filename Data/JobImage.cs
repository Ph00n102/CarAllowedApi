using System;

namespace CarAllowedApi.Data;

public class JobImage
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public byte[] ImageFile { get; set; }
    public int OpNoteDetailId { get; set; }
}
