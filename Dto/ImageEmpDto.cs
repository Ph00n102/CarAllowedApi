namespace CarAllowedApi.Dto;

    public class ImageEmpDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Nickname { get; set; }
        public string Empposition { get; set; }
        public string Tel { get; set; }
        public string FileName { get; set; }
        public string EmpStatusId { get; set; }
        public IFormFile ImageFile { get; set; } 
    }