// ในไฟล์ DTOs/ImageFileDto.cs
namespace CarAllowedApi.DTOs
{
    public class ImageFilesDto
    {
        public int Id { get; set; }
        public int JobRequestCarId { get; set; }
        public string FileName { get; set; }
    }
}