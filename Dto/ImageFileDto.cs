// ในไฟล์ DTOs/ImageFileDto.cs
namespace CarAllowedApi.DTOs
{
    public class ImageFileDto
    {
        public int Id { get; set; }
        public int JobRequestCarId { get; set; }
        public string FileName { get; set; }
        public byte[] ImageFile { get; set; }
    }
}