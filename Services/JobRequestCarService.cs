using System.Globalization;
using CarAllowedApi.Data;
using CarAllowedApi.Dto;
using Microsoft.EntityFrameworkCore;
using static CarAllowedApi.Services.JobRequestCarService;

namespace CarAllowedApi.Services;

public interface IJobRequestCarService
{
    Task<JobRequestCar[]> GetAllJobRequestCarsAsync();
    Task<JobRequestCarAllDayDto[]> GetAllDay1JobRequestCarsAsync();
    Task<JobRequestCarAllDayDto[]> GetAllEmpDayJobRequestCarsAsync(string name, int statusId);
    Task<JobRequestCarAllDayDto[]> GetAllEmptoDayJobRequestCarsAsync(string name);
    Task<IEnumerable<JobRequestCar>> GetAll1JobRequestCarsAsync();
    Task<IEnumerable<SearchResultDto>> GetAllSearchJobRequestCarsAsync();
    Task<IEnumerable<JobRequestCar>> GetAllDayJobRequestCarsAsync();
    Task<JobRequestCar?> GetJobRequestCarByIdAsync(int id);
    Task<JobRequestCar[]> GetJobRequestCarsByStatusAsync(int statusId);
    Task<JobRequestCarNoImDto?> GetJobRequestNoImByIdAsync(int id);
    Task<JobRequestCar> CreateJobRequestCarAsync(JobRequestCarDto dto);
    Task<JobRequestCar?> UpdateJobRequestCarAsync(int id, JobRequestCarUpDto dto);
    Task<JobRequestCar?> UpdateJobRequestCarPartialAsync(int id, JobRequestCarPartialUpdateDto dto);
    Task<JobRequestCar?> UpdateJobDistributeCarPartialAsync(int id, JobDistributeCarPartialDto dto);
    Task<JobRequestCar?> UpdateJobAcceptingAsync(int id, JobAcceptingDto dto);
    Task<JobRequestCarAllDayDto[]> GetAllListDayJobRequestCarsAsync(string name , int statusId);
    Task<JobRequestCarAllDayDto[]> GetAllListtoDayJobRequestCarsAsync(string name);

    // เปลี่ยน string เป็น int
    Task<IEnumerable<JobStatus>> GetAllJobStatusesAsync();
    Task<JobStatus?> GetJobStatusByIdAsync(int id); // เปลี่ยนจาก string เป็น int
    Task<JobStatus> CreateJobStatusAsync(JobStatusDto dto);
    Task<JobStatus?> UpdateJobStatusAsync(int id, JobStatusDto dto); // เปลี่ยนจาก string เป็น int
    Task<bool> DeleteJobStatusAsync(int id); // เปลี่ยนจาก string เป็น int
        // เพิ่มเมธอดสำหรับการจ่ายงานแบบมีเลขรัน
    Task<JobRequestCar?> DistributeJobWithNumberAsync(int id, JobDistributeCarFullDto dto);
    
    // เมธอดสำหรับอัพเดท Job Number
    Task<JobRequestCar?> UpdateJobNumberAsync(int id, string jobNumber);
    
    // เมธอดสำหรับดึงข้อมูลเลขรัน
    Task<JobNumberResultDto> GetJobNumberInfoForJobAsync(int jobId);
}

public class JobRequestCarService : IJobRequestCarService
{
    private readonly ApplicationDbContext _context;

    public JobRequestCarService(ApplicationDbContext context)
    {
        _context = context;
    }

    // ในส่วนของ service
    public async Task<JobRequestCar[]> GetAllJobRequestCarsAsync()
    {
        return await _context.JobRequestCars
            .Include(j => j.ImageFiles)
            .Include(j => j.JobStatus)
            .Include(j => j.ImageEmp)  // เพิ่ม Include ImageEmp
            .Include(j => j.Garage)    // เพิ่ม Include Garage
            .AsNoTracking()
            .ToArrayAsync();
    }
    public async Task<IEnumerable<JobRequestCar>> GetAll1JobRequestCarsAsync()
    {
        return await _context.JobRequestCars
            .Include(j => j.ImageFiles)
            .Include(j => j.JobStatus)
            .Include(j => j.ImageEmp)  // เพิ่ม Include ImageEmp
            .Include(j => j.Garage)
            .ToListAsync();
    }
    public async Task<IEnumerable<SearchResultDto>> GetAllSearchJobRequestCarsAsync()
    {
        return await _context.JobRequestCars
            .Include(j => j.JobStatus)
            .Include(j => j.Garage)
            .Select(g => new SearchResultDto
            {
                Id = g.Id,
                Requester = g.Requester,
                DepartmentId = g.DepartmentId,
                Position = g.Position, 
                District = g.District,
                Province = g.Province,
                AlongWith = g.AlongWith,
                For = g.For,
                Location = g.Location,
                PerApplicant = g.PerApplicant,
                Origin = g.Origin,
                Destination = g.Destination,
                Tel = g.Tel,
                PerPosition = g.PerPosition
            })
            .ToListAsync();
    }
    public async Task<IEnumerable<JobRequestCar>> GetAllDayJobRequestCarsAsync()
    {
        // ใช้ DateOnly.FromDateTime(DateTime.Today) แทน
        var today = DateOnly.FromDateTime(DateTime.Today);

        return await _context.JobRequestCars
            .Include(j => j.ImageFiles)
            .Include(j => j.JobStatus)
            .Include(j => j.ImageEmp)  // เพิ่ม Include ImageEmp
            .Include(j => j.Garage)
            .OrderBy(x => x.StartDate)
            .Where(j => (j.DateNow == today || j.StartDate == today) && !(j.JobStatusId == 3 || j.JobStatusId == 4))  // เปรียบเทียบ DateOnly โดยตรง
            .ToListAsync();
    }

    public async Task<JobRequestCarAllDayDto[]> GetAllDay1JobRequestCarsAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        return await _context.JobRequestCars
            .AsNoTracking()
            .Where(j => (j.DateNow == today || j.StartDate == today) && j.JobStatusId != 3 || j.JobStatusId != 4)
            .OrderBy(x => x.StartDate)  // เรียงตาม StartDate จากมากไปน้อย (วันที่ล่าสุดมาก่อน)
            .ThenByDescending(x => x.StartTime)   // ถ้า StartDate เท่ากัน ให้เรียงตามเวลา
            .ThenByDescending(x => x.Id)          // ถ้า StartDate และ StartTime เท่ากัน ให้เรียงตาม Id
            .Select(g => new JobRequestCarAllDayDto
            {
                Id = g.Id,
                DateNow = g.DateNow,
                TimeNow = g.TimeNow,
                EDateNow = g.EDateNow,
                ETimeNow = g.ETimeNow,
                Requester = g.Requester,
                DepartmentId = g.DepartmentId,
                Origin = g.Origin,
                Destination = g.Destination,
                StartDate = g.StartDate,
                StartTime = g.StartTime,
                EndDate = g.EndDate,
                EndTime = g.EndTime,
                JobStatusId = g.JobStatusId,
                ImageEmpId = g.ImageEmpId,
                GarageId = g.GarageId,
                NumPer = g.NumPer,
                Tel = g.Tel,
                Note = g.Note,
                Ot = g.Ot,
                MileageOut = g.MileageOut,
                MileageBack = g.MileageBack,
                NumOil = g.NumOil,
                Price = g.Price,
                IssueDate = g.IssueDate,
                IssueTime = g.IssueTime,
                ReturnDate = g.ReturnDate,
                JobNumber = g.JobNumber,
                ReturnTime = g.ReturnTime
            })
            .ToArrayAsync();
    }
   public async Task<JobRequestCarAllDayDto[]> GetAllEmpDayJobRequestCarsAsync(string name = null, int statusId = 0)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var query = _context.JobRequestCars
            .AsNoTracking()
            .Include(j => j.JobStatus)
            .Include(j => j.ImageEmp)
            .Include(j => j.Garage)
            .AsQueryable();

        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(j => EF.Functions.Like(j.ImageEmp.Name, $"%{name}%"));
        }
        
        if (statusId > 0)
        {
            query = query.Where(j => j.JobStatusId == statusId);
        }
        
        var result = await query
            .OrderBy(x => x.StartDate)
            .ToArrayAsync();
        
        // Map to DTO
        return result.Select(g => new JobRequestCarAllDayDto
        {
            Id = g.Id,
            DateNow = g.DateNow,
            TimeNow = g.TimeNow,
            EDateNow = g.EDateNow,
            ETimeNow = g.ETimeNow,
            Requester = g.Requester,
            DepartmentId = g.DepartmentId,
            Origin = g.Origin,
            Destination = g.Destination,
            StartDate = g.StartDate,
            StartTime = g.StartTime,
            EndDate = g.EndDate,
            EndTime = g.EndTime,
            JobStatusId = g.JobStatusId,
            ImageEmpId = g.ImageEmpId,
            GarageId = g.GarageId,
            NumPer = g.NumPer,
            Tel = g.Tel,
            Note = g.Note,
            Ot = g.Ot,
            MileageOut = g.MileageOut,
            MileageBack = g.MileageBack,
            NumOil = g.NumOil,
            Price = g.Price,
            IssueDate = g.IssueDate,
            IssueTime = g.IssueTime,
            ReturnDate = g.ReturnDate,
            JobNumber = g.JobNumber,
            ReturnTime = g.ReturnTime,
            
            // Include navigation properties
            JobStatus = g.JobStatus,
            ImageEmp = g.ImageEmp,
            Garage = g.Garage
        }).ToArray();
    }
   public async Task<JobRequestCarAllDayDto[]> GetAllEmptoDayJobRequestCarsAsync(string name = null)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var query = _context.JobRequestCars
            .AsNoTracking()
            .Include(j => j.JobStatus)
            .Include(j => j.ImageEmp)
            .Include(j => j.Garage)
            .Where(j => (j.DateNow == today || j.StartDate == today) && !(j.JobStatusId == 3 || j.JobStatusId == 4))
            .AsQueryable();

        // เพิ่มเงื่อนไขกรองตาม name ถ้าไม่ใช่ null หรือว่างเปล่า
        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(j => EF.Functions.Like(j.ImageEmp.Name, $"%{name}%"));
        }
        
        return await query
            .OrderBy(x => x.StartDate)
            .Select(g => new JobRequestCarAllDayDto
            {
                Id = g.Id,
                DateNow = g.DateNow,
                TimeNow = g.TimeNow,
                EDateNow = g.EDateNow,
                ETimeNow = g.ETimeNow,
                Requester = g.Requester,
                DepartmentId = g.DepartmentId,
                Origin = g.Origin,
                Destination = g.Destination,
                StartDate = g.StartDate,
                StartTime = g.StartTime,
                EndDate = g.EndDate,
                EndTime = g.EndTime,
                JobStatusId = g.JobStatusId,
                ImageEmpId = g.ImageEmpId,
                GarageId = g.GarageId,
                NumPer = g.NumPer,
                Tel = g.Tel,
                Note = g.Note,
                Ot = g.Ot,
                MileageOut = g.MileageOut,
                MileageBack = g.MileageBack,
                NumOil = g.NumOil,
                Price = g.Price,
                IssueDate = g.IssueDate,
                IssueTime = g.IssueTime,
                ReturnDate = g.ReturnDate,
                JobNumber = g.JobNumber,
                ReturnTime = g.ReturnTime,
                
                // เพิ่ม mapping สำหรับ navigation properties
                JobStatus = g.JobStatus,                     // เพิ่มบรรทัดนี้
                ImageEmp = g.ImageEmp,                       // เพิ่มบรรทัดนี้
                Garage = g.Garage                            // เพิ่มบรรทัดนี้
            })
            .ToArrayAsync();
    }
   public async Task<JobRequestCarAllDayDto[]> GetAllListDayJobRequestCarsAsync(string name = null, int statusId = 0)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var query = _context.JobRequestCars
            .AsNoTracking()
            .Include(j => j.JobStatus)
            .Include(j => j.ImageEmp)
            .Include(j => j.Garage)
            .AsQueryable();

        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(j => EF.Functions.Like(j.Requester, $"%{name}%"));
        }
        
        if (statusId > 0)
        {
            query = query.Where(j => j.JobStatusId == statusId);
        }
        
        var result = await query
            .OrderBy(x => x.StartDate)
            .ToArrayAsync();
        
        // Map to DTO
        return result.Select(g => new JobRequestCarAllDayDto
        {
            Id = g.Id,
            DateNow = g.DateNow,
            TimeNow = g.TimeNow,
            EDateNow = g.EDateNow,
            ETimeNow = g.ETimeNow,
            Requester = g.Requester,
            DepartmentId = g.DepartmentId,
            Origin = g.Origin,
            Destination = g.Destination,
            StartDate = g.StartDate,
            StartTime = g.StartTime,
            EndDate = g.EndDate,
            EndTime = g.EndTime,
            JobStatusId = g.JobStatusId,
            ImageEmpId = g.ImageEmpId,
            GarageId = g.GarageId,
            NumPer = g.NumPer,
            Tel = g.Tel,
            Note = g.Note,
            Ot = g.Ot,
            MileageOut = g.MileageOut,
            MileageBack = g.MileageBack,
            NumOil = g.NumOil,
            Price = g.Price,
            IssueDate = g.IssueDate,
            IssueTime = g.IssueTime,
            ReturnDate = g.ReturnDate,
            JobNumber = g.JobNumber,
            ReturnTime = g.ReturnTime,
            
            // Include navigation properties
            JobStatus = g.JobStatus,
            ImageEmp = g.ImageEmp,
            Garage = g.Garage
        }).ToArray();
    }
   public async Task<JobRequestCarAllDayDto[]> GetAllListtoDayJobRequestCarsAsync(string name = null)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var query = _context.JobRequestCars
            .AsNoTracking()
            .Include(j => j.JobStatus)
            .Include(j => j.ImageEmp)
            .Include(j => j.Garage)
            .Where(j => (j.DateNow == today || j.StartDate == today) && !(j.JobStatusId == 3 || j.JobStatusId == 4))
            .AsQueryable();

        // เพิ่มเงื่อนไขกรองตาม name ถ้าไม่ใช่ null หรือว่างเปล่า
         if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(j => EF.Functions.Like(j.Requester, $"%{name}%"));
        }
        
        return await query
            .OrderBy(x => x.StartDate)
            .Select(g => new JobRequestCarAllDayDto
            {
                Id = g.Id,
                DateNow = g.DateNow,
                TimeNow = g.TimeNow,
                EDateNow = g.EDateNow,
                ETimeNow = g.ETimeNow,
                Requester = g.Requester,
                DepartmentId = g.DepartmentId,
                Origin = g.Origin,
                Destination = g.Destination,
                StartDate = g.StartDate,
                StartTime = g.StartTime,
                EndDate = g.EndDate,
                EndTime = g.EndTime,
                JobStatusId = g.JobStatusId,
                ImageEmpId = g.ImageEmpId,
                GarageId = g.GarageId,
                NumPer = g.NumPer,
                Tel = g.Tel,
                Note = g.Note,
                Ot = g.Ot,
                MileageOut = g.MileageOut,
                MileageBack = g.MileageBack,
                NumOil = g.NumOil,
                Price = g.Price,
                IssueDate = g.IssueDate,
                IssueTime = g.IssueTime,
                ReturnDate = g.ReturnDate,
                JobNumber = g.JobNumber,
                ReturnTime = g.ReturnTime,
                
                // เพิ่ม mapping สำหรับ navigation properties
                JobStatus = g.JobStatus,                     // เพิ่มบรรทัดนี้
                ImageEmp = g.ImageEmp,                       // เพิ่มบรรทัดนี้
                Garage = g.Garage                            // เพิ่มบรรทัดนี้
            })
            .ToArrayAsync();
    }


    // เพิ่ม method สำหรับดึงข้อมูลตามเงื่อนไขต่างๆ (ถ้าต้องการ)
    public async Task<JobRequestCar[]> GetJobRequestCarsByStatusAsync(int statusId)
    {
        return await _context.JobRequestCars
            .Where(j => j.JobStatusId == statusId)
            .Include(j => j.ImageFiles)
            .Include(j => j.JobStatus)
            .Include(j => j.ImageEmp)
            .Include(j => j.Garage)
            .AsNoTracking()
            .ToArrayAsync();
    }

    public async Task<JobRequestCar?> GetJobRequestCarByIdAsync(int id)
    {
        return await _context.JobRequestCars
            .Include(j => j.ImageFiles)      // ไฟล์แนบ
            .Include(j => j.JobStatus)       // สถานะ
            .Include(j => j.ImageEmp)        // คนขับ (ถ้ามี relation)
            .Include(j => j.Garage)          // รถ (ถ้ามี relation)
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == id);
    }
    public async Task<JobRequestCarNoImDto?> GetJobRequestNoImByIdAsync(int id)
    {
        return await _context.JobRequestCars
            .Where(j => j.Id == id)
            .Select(j => new JobRequestCarNoImDto
            {
                Id = j.Id,
                DateNow = j.DateNow,
                TimeNow = j.TimeNow,
                EDateNow = j.EDateNow,
                ETimeNow = j.ETimeNow,
                Requester = j.Requester,
                DepartmentId = j.DepartmentId,
                Origin = j.Origin,
                PerApplicant = j.PerApplicant,
                PerPosition = j.PerPosition,
                Destination = j.Destination,
                AlongWith = j.AlongWith,
                For = j.For,
                Location = j.Location,
                LocationTime = j.LocationTime,
                StartDate = j.StartDate,
                StartTime = j.StartTime,
                EndDate = j.EndDate,
                EndTime = j.EndTime,
                JDDate = j.JDDate,
                JDTime = j.JDTime,
                StatusDate = j.StatusDate,
                StatusTime = j.StatusTime,
                JobStatusId = j.JobStatusId,
                JobStatusName = j.JobStatus != null ? j.JobStatus.Status : null,
                ImageEmpId = j.ImageEmpId,
                ImageEmpName = j.ImageEmp != null ? j.ImageEmp.Name : null,
                GarageId = j.GarageId,
                Cartype = j.Garage != null ? j.Garage.Cartype : null,
                GarageCarRegistration = j.Garage != null ? j.Garage.CarRegistration : null,
                NumPer = j.NumPer,
                Tel = j.Tel,
                Note = j.Note,
                Ot = j.Ot,
                MileageOut = j.MileageOut,
                MileageBack = j.MileageBack,
                NumOil = j.NumOil,
                Price = j.Price,
                IssueDate = j.IssueDate,
                IssueTime = j.IssueTime,
                ReturnDate = j.ReturnDate,
                Position = j.Position,
                District = j.District,
                Province = j.Province,
                UpNDate = j.UpNDate,
                UpNTime = j.UpNTime,
                JobNumber = j.JobNumber,
                ReturnTime = j.ReturnTime
            })
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    // public async Task<JobRequestCar> CreateJobRequestCarAsync(JobRequestCarDto dto)
    // {
    //     if (dto == null)
    //         throw new ArgumentNullException(nameof(dto), "Input data is null");

    //     int jobStatusId;

    //     if (dto.JobStatusId.HasValue)
    //     {
    //         var existingStatus = await _context.JobStatuses
    //             .FindAsync(dto.JobStatusId.Value);

    //         if (existingStatus == null)
    //         {
    //             throw new ArgumentException($"JobStatusId '{dto.JobStatusId}' not found in database");
    //         }

    //         jobStatusId = dto.JobStatusId.Value;
    //     }
    //     else
    //     {
    //         var defaultJobStatus = await _context.JobStatuses
    //             .FirstOrDefaultAsync(s => s.Status == "รอจ่ายงาน");

    //         if (defaultJobStatus == null)
    //         {
    //             throw new InvalidOperationException("Default job status 'รอจ่ายงาน' not found in database");
    //         }

    //         jobStatusId = defaultJobStatus.Id;
    //     }

    //     // แปลง ImageFiles จาก IFormFile
    //     var jobImages = new List<JobImage>();

    //     if (dto.ImageFiles != null && dto.ImageFiles.Count > 0)
    //     {
    //         foreach (var imageFile in dto.ImageFiles)
    //         {
    //             if (imageFile != null && imageFile.Length > 0)
    //             {
    //                 byte[] imageBytes;
    //                 using (var ms = new MemoryStream())
    //                 {
    //                     await imageFile.CopyToAsync(ms);
    //                     imageBytes = ms.ToArray();
    //                 }

    //                 jobImages.Add(new JobImage
    //                 {
    //                     FileName = imageFile.FileName ?? string.Empty,
    //                     ImageFile = imageBytes,
    //                     OpNoteDetailId = 0
    //                 });
    //             }
    //         }
    //     }

    //     var jobRequestCar = new JobRequestCar
    //     {
    //         DateNow = dto.DateNow,
    //         TimeNow = dto.TimeNow,
    //         EDateNow = dto.EDateNow,
    //         ETimeNow = dto.ETimeNow,
    //         Requester = dto.Requester ?? string.Empty,
    //         DepartmentId = dto.DepartmentId ?? string.Empty,
    //         Origin = dto.Origin ?? string.Empty,
    //         Destination = dto.Destination ?? string.Empty,
    //         StartDate = dto.StartDate,
    //         StartTime = dto.StartTime,
    //         EndDate = dto.EndDate,
    //         EndTime = dto.EndTime,
    //         NumPer = dto.NumPer ?? 0,
    //         Tel = dto.Tel ?? string.Empty,
    //         Note = dto.Note ?? string.Empty,
    //         JobStatusId = jobStatusId,
    //         ImageEmpId = dto.ImageEmpId,
    //         GarageId = dto.GarageId,
    //         ImageFiles = jobImages
    //     };

    //     _context.JobRequestCars.Add(jobRequestCar);
    //     await _context.SaveChangesAsync();

    //     await _context.Entry(jobRequestCar)
    //         .Collection(j => j.ImageFiles)
    //         .LoadAsync();

    //     return jobRequestCar;
    // }

    // public async Task<JobRequestCar?> UpdateJobRequestCarAsync(int id, JobRequestCarDto dto)
    // {
    //     if (dto == null)
    //         throw new ArgumentNullException(nameof(dto), "Input data is null");

    //     // Find existing job request car
    //     var existingJobRequestCar = await _context.JobRequestCars
    //         .Include(j => j.ImageFiles)
    //         .FirstOrDefaultAsync(j => j.Id == id);

    //     if (existingJobRequestCar == null)
    //         return null;

    //     // Validate JobStatusId if provided
    //     int jobStatusId = existingJobRequestCar.JobStatusId;

    //     if (dto.JobStatusId.HasValue)
    //     {
    //         var existingStatus = await _context.JobStatuses
    //             .FindAsync(dto.JobStatusId.Value);

    //         if (existingStatus == null)
    //         {
    //             throw new ArgumentException($"JobStatusId '{dto.JobStatusId}' not found in database");
    //         }

    //         jobStatusId = dto.JobStatusId.Value;
    //     }

    //     // Update properties
    //     existingJobRequestCar.DateNow = dto.DateNow;
    //     existingJobRequestCar.TimeNow = dto.TimeNow;
    //     existingJobRequestCar.EDateNow = dto.EDateNow;
    //     existingJobRequestCar.ETimeNow = dto.ETimeNow;
    //     existingJobRequestCar.Requester = dto.Requester ?? string.Empty;
    //     existingJobRequestCar.DepartmentId = dto.DepartmentId ?? string.Empty;
    //     existingJobRequestCar.Origin = dto.Origin ?? string.Empty;
    //     existingJobRequestCar.Destination = dto.Destination ?? string.Empty;
    //     existingJobRequestCar.StartDate = dto.StartDate;
    //     existingJobRequestCar.StartTime = dto.StartTime;
    //     existingJobRequestCar.EndDate = dto.EndDate;
    //     existingJobRequestCar.EndTime = dto.EndTime;
    //     existingJobRequestCar.NumPer = dto.NumPer ?? 0;
    //     existingJobRequestCar.Tel = dto.Tel ?? string.Empty;
    //     existingJobRequestCar.Note = dto.Note ?? string.Empty;
    //     existingJobRequestCar.JobStatusId = jobStatusId;
    //     existingJobRequestCar.ImageEmpId = dto.ImageEmpId;
    //     existingJobRequestCar.GarageId = dto.GarageId;

    //     // Handle image updates
    //     if (dto.ImageFiles != null)
    //     {
    //         // Remove existing images
    //         if (existingJobRequestCar.ImageFiles != null)
    //         {
    //             _context.JobImages.RemoveRange(existingJobRequestCar.ImageFiles);
    //         }

    //         // Add new images
    //         // var jobImages = new List<JobImage>();
    //         // foreach (var imgDto in dto.ImageFiles)
    //         // {
    //         //     byte[] imageBytes = Array.Empty<byte>();

    //         //     if (imgDto.ImageFile != null && imgDto.ImageFile.Length > 0)
    //         //     {
    //         //         imageBytes = imgDto.ImageFile;
    //         //     }

    //         //     jobImages.Add(new JobImage
    //         //     {
    //         //         FileName = imgDto.FileName ?? string.Empty,
    //         //         ImageFile = imageBytes,
    //         //         OpNoteDetailId = 0
    //         //     });
    //         // }

    //         // existingJobRequestCar.ImageFiles = jobImages;
    //     }

    //     await _context.SaveChangesAsync();

    //     // Reload with related data
    //     await _context.Entry(existingJobRequestCar)
    //         .Collection(j => j.ImageFiles)
    //         .LoadAsync();

    //     await _context.Entry(existingJobRequestCar)
    //         .Reference(j => j.JobStatus)
    //         .LoadAsync();

    //     return existingJobRequestCar;
    // }
    // public async Task<JobRequestCar> CreateJobRequestCarAsync(JobRequestCarDto dto)
    // {
    //     if (dto == null)
    //         throw new ArgumentNullException(nameof(dto), "Input data is null");

    //     try
    //     {
    //         int jobStatusId;
    //         int? garageId = null;
    //         int? imageEmpId = null;

    //         // ตรวจสอบ JobStatusId
    //         if (dto.JobStatusId.HasValue)
    //         {
    //             var existingStatus = await _context.JobStatuses
    //                 .FindAsync(dto.JobStatusId.Value);

    //             if (existingStatus == null)
    //             {
    //                 throw new ArgumentException($"JobStatusId '{dto.JobStatusId}' not found in database");
    //             }

    //             jobStatusId = dto.JobStatusId.Value;
    //         }
    //         else
    //         {
    //             var defaultJobStatus = await _context.JobStatuses
    //                 .FirstOrDefaultAsync(s => s.Status == "รอจ่ายงาน");

    //             if (defaultJobStatus == null)
    //             {
    //                 throw new InvalidOperationException("Default job status 'รอจ่ายงาน' not found in database");
    //             }

    //             jobStatusId = defaultJobStatus.Id;
    //         }

    //         // ตรวจสอบ GarageId - ถ้ามีค่าให้ตรวจสอบว่ามีใน database
    //         if (dto.GarageId.HasValue && dto.GarageId.Value > 0)
    //         {
    //             var existingGarage = await _context.Garages
    //                 .FindAsync(dto.GarageId.Value);

    //             if (existingGarage == null)
    //             {
    //                 throw new ArgumentException($"GarageId '{dto.GarageId}' not found in database");
    //             }
    //             garageId = dto.GarageId.Value;
    //         }
    //         // ถ้าไม่ส่งค่ามาหรือส่ง 0 ให้เป็น null ได้
    //         else if (dto.GarageId.HasValue && dto.GarageId.Value == 0)
    //         {
    //             garageId = null; // อนุญาตให้เป็น null ได้
    //         }
    //         // ไม่ส่งค่าเลยก็เป็น null
    //         else
    //         {
    //             garageId = null;
    //         }

    //         // ตรวจสอบ ImageEmpId - ถ้ามีค่าให้ตรวจสอบว่ามีใน database
    //         if (dto.ImageEmpId.HasValue && dto.ImageEmpId.Value > 0)
    //         {
    //             var existingImageEmp = await _context.ImageEmps
    //                 .FindAsync(dto.ImageEmpId.Value);

    //             if (existingImageEmp == null)
    //             {
    //                 throw new ArgumentException($"ImageEmpId '{dto.ImageEmpId}' not found in database");
    //             }
    //             imageEmpId = dto.ImageEmpId.Value;
    //         }
    //         // ถ้าไม่ส่งค่ามาหรือส่ง 0 ให้เป็น null ได้
    //         else if (dto.ImageEmpId.HasValue && dto.ImageEmpId.Value == 0)
    //         {
    //             imageEmpId = null; // อนุญาตให้เป็น null ได้
    //         }
    //         // ไม่ส่งค่าเลยก็เป็น null
    //         else
    //         {
    //             imageEmpId = null;
    //         }

    //         // กำหนดค่าเริ่มต้นสำหรับวันที่และเวลา
    //         if (string.IsNullOrEmpty(dto.DateNow))
    //         {
    //             dto.DateNow = DateOnly.FromDateTime(DateTime.Now).ToString("yyyy-MM-dd");
    //         }

    //         if (string.IsNullOrEmpty(dto.TimeNow))
    //         {
    //             dto.TimeNow = TimeOnly.FromDateTime(DateTime.Now).ToString("HH:mm");
    //         }

    //         if (string.IsNullOrEmpty(dto.EDateNow))
    //         {
    //             dto.EDateNow = DateOnly.FromDateTime(DateTime.Now).ToString("yyyy-MM-dd");
    //         }

    //         if (string.IsNullOrEmpty(dto.ETimeNow))
    //         {
    //             dto.ETimeNow = TimeOnly.FromDateTime(DateTime.Now).ToString("HH:mm");
    //         }

    //         // ตรวจสอบ required fields
    //         if (string.IsNullOrEmpty(dto.StartDate))
    //             throw new ArgumentException("StartDate is required");
    //         if (string.IsNullOrEmpty(dto.StartTime))
    //             throw new ArgumentException("StartTime is required");
    //         if (string.IsNullOrEmpty(dto.EndDate))
    //             throw new ArgumentException("EndDate is required");
    //         if (string.IsNullOrEmpty(dto.EndTime))
    //             throw new ArgumentException("EndTime is required");
    //         if (string.IsNullOrEmpty(dto.LocationTime))
    //             throw new ArgumentException("LocationTime is required");

    //         // แปลงวันที่และเวลา
    //         DateOnly dateNow = ParseDate(dto.DateNow);
    //         TimeOnly timeNow = ParseTime(dto.TimeNow);
    //         DateOnly eDateNow = ParseDate(dto.EDateNow);
    //         TimeOnly eTimeNow = ParseTime(dto.ETimeNow);
    //         DateOnly startDate = ParseDate(dto.StartDate);
    //         TimeOnly startTime = ParseTime(dto.StartTime);
    //         DateOnly endDate = ParseDate(dto.EndDate);
    //         TimeOnly endTime = ParseTime(dto.EndTime);
    //         TimeOnly locationTime = ParseTime(dto.LocationTime);

    //         // แปลง ImageFiles จาก IFormFile
    //         var jobImages = new List<JobImage>();

    //         if (dto.ImageFiles != null && dto.ImageFiles.Count > 0)
    //         {
    //             foreach (var imageFile in dto.ImageFiles)
    //             {
    //                 if (imageFile != null && imageFile.Length > 0)
    //                 {
    //                     byte[] imageBytes;
    //                     using (var ms = new MemoryStream())
    //                     {
    //                         await imageFile.CopyToAsync(ms);
    //                         imageBytes = ms.ToArray();
    //                     }

    //                     jobImages.Add(new JobImage
    //                     {
    //                         FileName = imageFile.FileName ?? "unknown.jpg",
    //                         ImageFile = imageBytes,
    //                         OpNoteDetailId = 0
    //                     });
    //                 }
    //             }
    //         }

    //         var jobRequestCar = new JobRequestCar
    //         {
    //             DateNow = dateNow,
    //             TimeNow = timeNow,
    //             EDateNow = eDateNow,
    //             ETimeNow = eTimeNow,
    //             Requester = dto.Requester ?? string.Empty,
    //             DepartmentId = dto.DepartmentId ?? string.Empty,
    //             PerApplicant = dto.PerApplicant ?? string.Empty,
    //             PerPosition = dto.PerPosition ?? string.Empty,
    //             Position = dto.Position ?? string.Empty,
    //             District = dto.District ?? string.Empty,
    //             Province = dto.Province ?? string.Empty,
    //             Origin = dto.Origin ?? string.Empty,
    //             Destination = dto.Destination ?? string.Empty,
    //             AlongWith = dto.AlongWith ?? string.Empty,
    //             For = dto.For ?? string.Empty,
    //             Location = dto.Location ?? string.Empty,
    //             StartDate = startDate,
    //             StartTime = startTime,
    //             EndDate = endDate,
    //             EndTime = endTime,
    //             LocationTime = locationTime,
    //             NumPer = dto.NumPer ?? 0,
    //             Tel = dto.Tel ?? string.Empty,
    //             Note = dto.Note ?? "",
    //             JobStatusId = jobStatusId,
    //             ImageEmpId = imageEmpId,    // สามารถเป็น null ได้
    //             GarageId = garageId,        // สามารถเป็น null ได้
    //             ImageFiles = jobImages
    //         };

    //         _context.JobRequestCars.Add(jobRequestCar);
    //         await _context.SaveChangesAsync();

    //         await _context.Entry(jobRequestCar)
    //             .Collection(j => j.ImageFiles)
    //             .LoadAsync();

    //         return jobRequestCar;
    //     }
    //     catch (DbUpdateException dbEx)
    //     {
    //         var innerException = dbEx.InnerException?.Message ?? dbEx.Message;

    //         // เพิ่มการตรวจสอบ specific error
    //         if (innerException.Contains("foreign key constraint fails"))
    //         {
    //             throw new Exception($"Foreign key constraint error. Please check if GarageId or ImageEmpId exist in their tables.");
    //         }

    //         throw new Exception($"Database error: {innerException}");
    //     }
    //     catch (Exception ex)
    //     {
    //         throw new Exception($"Error creating job request: {ex.Message}", ex);
    //     }
    // }
    public async Task<JobRequestCar> CreateJobRequestCarAsync(JobRequestCarDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto), "Input data is null");

        try
        {
            int jobStatusId;
            int? garageId = null;
            int? imageEmpId = null;

            // ตรวจสอบ JobStatusId
            if (dto.JobStatusId.HasValue)
            {
                var existingStatus = await _context.JobStatuses
                    .FindAsync(dto.JobStatusId.Value);

                if (existingStatus == null)
                {
                    throw new ArgumentException($"JobStatusId '{dto.JobStatusId}' not found in database");
                }

                jobStatusId = dto.JobStatusId.Value;
            }
            else
            {
                var defaultJobStatus = await _context.JobStatuses
                    .FirstOrDefaultAsync(s => s.Status == "รอจ่ายงาน");

                if (defaultJobStatus == null)
                {
                    throw new InvalidOperationException("Default job status 'รอจ่ายงาน' not found in database");
                }

                jobStatusId = defaultJobStatus.Id;
            }

            // ตรวจสอบ GarageId - ถ้ามีค่าให้ตรวจสอบว่ามีใน database
            if (dto.GarageId.HasValue && dto.GarageId.Value > 0)
            {
                var existingGarage = await _context.Garages
                    .FindAsync(dto.GarageId.Value);

                if (existingGarage == null)
                {
                    throw new ArgumentException($"GarageId '{dto.GarageId}' not found in database");
                }
                garageId = dto.GarageId.Value;
            }
            // ถ้าไม่ส่งค่ามาหรือส่ง 0 ให้เป็น null ได้
            else if (dto.GarageId.HasValue && dto.GarageId.Value == 0)
            {
                garageId = null; // อนุญาตให้เป็น null ได้
            }
            // ไม่ส่งค่าเลยก็เป็น null
            else
            {
                garageId = null;
            }

            // ตรวจสอบ ImageEmpId - ถ้ามีค่าให้ตรวจสอบว่ามีใน database
            if (dto.ImageEmpId.HasValue && dto.ImageEmpId.Value > 0)
            {
                var existingImageEmp = await _context.ImageEmps
                    .FindAsync(dto.ImageEmpId.Value);

                if (existingImageEmp == null)
                {
                    throw new ArgumentException($"ImageEmpId '{dto.ImageEmpId}' not found in database");
                }
                imageEmpId = dto.ImageEmpId.Value;
            }
            // ถ้าไม่ส่งค่ามาหรือส่ง 0 ให้เป็น null ได้
            else if (dto.ImageEmpId.HasValue && dto.ImageEmpId.Value == 0)
            {
                imageEmpId = null; // อนุญาตให้เป็น null ได้
            }
            // ไม่ส่งค่าเลยก็เป็น null
            else
            {
                imageEmpId = null;
            }

            // กำหนดค่าเริ่มต้นสำหรับวันที่และเวลา
            if (string.IsNullOrEmpty(dto.DateNow))
            {
                dto.DateNow = DateOnly.FromDateTime(DateTime.Now).ToString("yyyy-MM-dd");
            }

            if (string.IsNullOrEmpty(dto.TimeNow))
            {
                dto.TimeNow = TimeOnly.FromDateTime(DateTime.Now).ToString("HH:mm");
            }

            if (string.IsNullOrEmpty(dto.EDateNow))
            {
                dto.EDateNow = DateOnly.FromDateTime(DateTime.Now).ToString("yyyy-MM-dd");
            }

            if (string.IsNullOrEmpty(dto.ETimeNow))
            {
                dto.ETimeNow = TimeOnly.FromDateTime(DateTime.Now).ToString("HH:mm");
            }

            // ตรวจสอบ required fields
            if (string.IsNullOrEmpty(dto.StartDate))
                throw new ArgumentException("StartDate is required");
            if (string.IsNullOrEmpty(dto.StartTime))
                throw new ArgumentException("StartTime is required");
            if (string.IsNullOrEmpty(dto.EndDate))
                throw new ArgumentException("EndDate is required");
            if (string.IsNullOrEmpty(dto.EndTime))
                throw new ArgumentException("EndTime is required");
            if (string.IsNullOrEmpty(dto.LocationTime))
                throw new ArgumentException("LocationTime is required");

            // แปลงวันที่และเวลา โดยแปลงปีจาก พ.ศ. เป็น ค.ศ. หากจำเป็น
            DateOnly dateNow = ParseDateAndConvertYear(dto.DateNow);
            TimeOnly timeNow = ParseTime(dto.TimeNow);
            DateOnly eDateNow = ParseDateAndConvertYear(dto.EDateNow);
            TimeOnly eTimeNow = ParseTime(dto.ETimeNow);
            DateOnly startDate = ParseDateAndConvertYear(dto.StartDate);
            TimeOnly startTime = ParseTime(dto.StartTime);
            DateOnly endDate = ParseDateAndConvertYear(dto.EndDate);
            TimeOnly endTime = ParseTime(dto.EndTime);
            TimeOnly locationTime = ParseTime(dto.LocationTime);

            // แปลง ImageFiles จาก IFormFile
            var jobImages = new List<JobImage>();

            if (dto.ImageFiles != null && dto.ImageFiles.Count > 0)
            {
                foreach (var imageFile in dto.ImageFiles)
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        byte[] imageBytes;
                        using (var ms = new MemoryStream())
                        {
                            await imageFile.CopyToAsync(ms);
                            imageBytes = ms.ToArray();
                        }

                        jobImages.Add(new JobImage
                        {
                            FileName = imageFile.FileName ?? "unknown.jpg",
                            ImageFile = imageBytes,
                            OpNoteDetailId = 0
                        });
                    }
                }
            }

            var jobRequestCar = new JobRequestCar
            {
                DateNow = dateNow,
                TimeNow = timeNow,
                EDateNow = eDateNow,
                ETimeNow = eTimeNow,
                Requester = dto.Requester ?? string.Empty,
                DepartmentId = dto.DepartmentId ?? string.Empty,
                PerApplicant = dto.PerApplicant ?? string.Empty,
                PerPosition = dto.PerPosition ?? string.Empty,
                Position = dto.Position ?? string.Empty,
                District = dto.District ?? string.Empty,
                Province = dto.Province ?? string.Empty,
                Origin = dto.Origin ?? string.Empty,
                Destination = dto.Destination ?? string.Empty,
                AlongWith = dto.AlongWith ?? string.Empty,
                For = dto.For ?? string.Empty,
                Location = dto.Location ?? string.Empty,
                StartDate = startDate,
                StartTime = startTime,
                EndDate = endDate,
                EndTime = endTime,
                LocationTime = locationTime,
                NumPer = dto.NumPer ?? 0,
                Tel = dto.Tel ?? string.Empty,
                Note = dto.Note ?? "",
                JobStatusId = jobStatusId,
                ImageEmpId = imageEmpId,    // สามารถเป็น null ได้
                GarageId = garageId,        // สามารถเป็น null ได้
                ImageFiles = jobImages
            };

            _context.JobRequestCars.Add(jobRequestCar);
            await _context.SaveChangesAsync();

            await _context.Entry(jobRequestCar)
                .Collection(j => j.ImageFiles)
                .LoadAsync();

            return jobRequestCar;
        }
        catch (DbUpdateException dbEx)
        {
            var innerException = dbEx.InnerException?.Message ?? dbEx.Message;

            // เพิ่มการตรวจสอบ specific error
            if (innerException.Contains("foreign key constraint fails"))
            {
                throw new Exception($"Foreign key constraint error. Please check if GarageId or ImageEmpId exist in their tables.");
            }

            throw new Exception($"Database error: {innerException}");
        }
        catch (Exception ex)
        {
            throw new Exception($"Error creating job request: {ex.Message}", ex);
        }
    }

    // เพิ่มเมธอดสำหรับแปลงวันที่และแปลงปี
    private DateOnly ParseDateAndConvertYear(string dateString)
    {
        try
        {
            // ลองแปลงด้วยรูปแบบมาตรฐานก่อน
            if (DateOnly.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly date))
            {
                return date;
            }
            
            // ถ้าไม่ได้ ให้ลองแยกส่วนและตรวจสอบ
            var parts = dateString.Split('-');
            if (parts.Length == 3 && int.TryParse(parts[0], out int year))
            {
                // ตรวจสอบว่าปีเป็น พ.ศ. หรือค.ศ.
                // ถ้าปีอยู่ในช่วง พ.ศ. (มากกว่า 2400)
                if (year > 2400 && year < 2500)  // หรือใช้ year > 2400
                {
                    // แปลงจาก พ.ศ. เป็น ค.ศ.
                    year = year - 543;
                    string convertedDateString = $"{year}-{parts[1]}-{parts[2]}";
                    
                    if (DateOnly.TryParse(convertedDateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly convertedDate))
                    {
                        return convertedDate;
                    }
                }
                else if (year > 1900 && year < 2100)  // ค.ศ. ในช่วงปัจจุบัน
                {
                    // ลองแปลงใหม่ด้วยรูปแบบที่ชัดเจน
                    string formattedDate = $"{year}-{parts[1]}-{parts[2]}";
                    if (DateOnly.TryParse(formattedDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly christianDate))
                    {
                        return christianDate;
                    }
                }
            }
            
            throw new ArgumentException($"Invalid date format: {dateString}");
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Error parsing date '{dateString}': {ex.Message}");
        }
    }

    // หรือถ้าต้องการเมธอดที่ยืดหยุ่นมากขึ้น สามารถใช้แบบนี้
    private DateOnly ParseDateAndConvertYearV2(string dateString)
    {
        // รองรับรูปแบบวันที่หลายรูปแบบ
        string[] dateFormats = new[] 
        {
            "yyyy-MM-dd",
            "dd/MM/yyyy",
            "dd-MM-yyyy",
            "yyyy/MM/dd",
            "dd/MM/yy",
            "dd-MM-yy",
            "yy-MM-dd"
        };
        
        // ลองแปลงด้วยรูปแบบมาตรฐานก่อน
        if (DateOnly.TryParseExact(dateString, dateFormats, 
            System.Globalization.CultureInfo.InvariantCulture, 
            System.Globalization.DateTimeStyles.None, out DateOnly date))
        {
            // ตรวจสอบว่าปีเป็น พ.ศ. หรือไม่
            if (date.Year > 2500)
            {
                // แปลงเป็น ค.ศ.
                return new DateOnly(date.Year - 543, date.Month, date.Day);
            }
            return date;
        }
        
        // ถ้าไม่สำเร็จ ลองตรวสอบว่าเป็นปี พ.ศ. ที่อาจมีรูปแบบแปลกๆ
        // แยกส่วนวันที่
        var separators = new char[] { '-', '/', '.', ' ' };
        var parts = dateString.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        
        if (parts.Length >= 3)
        {
            if (int.TryParse(parts[0], out int year) && 
                int.TryParse(parts[1], out int month) && 
                int.TryParse(parts[2], out int day))
            {
                // ถ้าปีเป็น พ.ศ.
                if (year > 2500)
                {
                    year -= 543;
                }
                // ถ้าปีเป็น 2 หลัก
                else if (year < 100)
                {
                    year = year < 30 ? 2000 + year : 1900 + year;
                }
                
                try
                {
                    return new DateOnly(year, month, day);
                }
                catch
                {
                    // ลองสลับลำดับเป็น วัน/เดือน/ปี
                    if (int.TryParse(parts[2], out year) && 
                        int.TryParse(parts[1], out month) && 
                        int.TryParse(parts[0], out day))
                    {
                        if (year > 2500) year -= 543;
                        else if (year < 100) year = year < 30 ? 2000 + year : 1900 + year;
                        
                        return new DateOnly(year, month, day);
                    }
                }
            }
        }
        
        throw new ArgumentException($"Invalid date format: {dateString}");
    }

    // เมธอดเดิมสำหรับแปลงเวลา (ไม่มีการเปลี่ยนแปลง)
    // private TimeOnly ParseTime(string timeString)
    // {
    //     try
    //     {
    //         if (TimeOnly.TryParse(timeString, out TimeOnly time))
    //         {
    //             return time;
    //         }
            
    //         throw new ArgumentException($"Invalid time format: {timeString}");
    //     }
    //     catch (Exception ex)
    //     {
    //         throw new ArgumentException($"Error parsing time '{timeString}': {ex.Message}");
    //     }
    // }
    public async Task<JobRequestCar?> UpdateJobRequestCarAsync(int id, JobRequestCarUpDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto), "Input data is null");

        // หาข้อมูลที่มีอยู่จากฐานข้อมูล
        var existingJobRequestCar = await _context.JobRequestCars
            .Include(j => j.ImageFiles) // รวมข้อมูลรูปภาพเดิม
            .FirstOrDefaultAsync(j => j.Id == id);

        if (existingJobRequestCar == null)
        {
            throw new KeyNotFoundException($"JobRequestCar with ID {id} not found");
        }

        // ตรวจสอบและกำหนด JobStatusId
        // int jobStatusId;

        // if (dto.JobStatusId.HasValue)
        // {
        //     var existingStatus = await _context.JobStatuses
        //         .FindAsync(dto.JobStatusId.Value);

        //     if (existingStatus == null)
        //     {
        //         throw new ArgumentException($"JobStatusId '{dto.JobStatusId}' not found in database");
        //     }

        //     jobStatusId = dto.JobStatusId.Value;
        // }
        // else
        // {
        //     // ถ้าไม่ได้ส่ง JobStatusId มา ใช้ค่าเดิม
        //     jobStatusId = existingJobRequestCar.JobStatusId;
        // }

        // อัพเดทรูปภาพ
        if (dto.ImageFiles != null && dto.ImageFiles.Count > 0)
        {
            // ลบรูปภาพเดิมทั้งหมด
            if (existingJobRequestCar.ImageFiles != null && existingJobRequestCar.ImageFiles.Any())
            {
                _context.JobImages.RemoveRange(existingJobRequestCar.ImageFiles);
            }

            // เพิ่มรูปภาพใหม่
            var jobImages = new List<JobImage>();
            foreach (var imageFile in dto.ImageFiles)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    byte[] imageBytes;
                    using (var ms = new MemoryStream())
                    {
                        await imageFile.CopyToAsync(ms);
                        imageBytes = ms.ToArray();
                    }

                    jobImages.Add(new JobImage
                    {
                        FileName = imageFile.FileName ?? string.Empty,
                        ImageFile = imageBytes,
                        OpNoteDetailId = 0
                    });
                }
            }
            existingJobRequestCar.ImageFiles = jobImages;
        }

        // แปลงวันที่และเวลา
        // กำหนดค่าเริ่มต้นถ้าวันที่/เวลาเป็น null
        // if (string.IsNullOrEmpty(dto.DateNow))
        // {
        //     dto.DateNow = DateOnly.FromDateTime(DateTime.Now).ToString("yyyy-MM-dd");
        // }

        // if (string.IsNullOrEmpty(dto.TimeNow))
        // {
        //     dto.TimeNow = TimeOnly.FromDateTime(DateTime.Now).ToString("HH:mm");
        // }

        // if (string.IsNullOrEmpty(dto.EDateNow))
        // {
        //     dto.EDateNow = DateOnly.FromDateTime(DateTime.Now).ToString("yyyy-MM-dd");
        // }

        // if (string.IsNullOrEmpty(dto.ETimeNow))
        // {
        //     dto.ETimeNow = TimeOnly.FromDateTime(DateTime.Now).ToString("HH:mm");
        // }

        // แปลง string เป็น DateOnly และ TimeOnly ด้วย format ที่ถูกต้อง
        try
        {
            // ใช้ TryParseExact เพื่อ parse ด้วย format ที่กำหนด
            // DateOnly dateNow = ParseDate(dto.DateNow);
            // TimeOnly timeNow = ParseTime(dto.TimeNow);
            // DateOnly eDateNow = ParseDate(dto.EDateNow);
            // TimeOnly eTimeNow = ParseTime(dto.ETimeNow);
            DateOnly startDate = ParseDate(dto.StartDate);
            TimeOnly startTime = ParseTime(dto.StartTime);
            TimeOnly locationTime = ParseTime(dto.LocationTime);
            DateOnly endDate = ParseDate(dto.EndDate);
            TimeOnly endTime = ParseTime(dto.EndTime);

            // อัพเดทข้อมูลทั้งหมด
            // existingJobRequestCar.DateNow = dateNow;
            // existingJobRequestCar.TimeNow = timeNow;
            // existingJobRequestCar.EDateNow = eDateNow;
            // existingJobRequestCar.ETimeNow = eTimeNow;
            existingJobRequestCar.Requester = dto.Requester ?? string.Empty;
            existingJobRequestCar.DepartmentId = dto.DepartmentId ?? string.Empty;
            existingJobRequestCar.AlongWith = dto.AlongWith ?? string.Empty;
            existingJobRequestCar.For = dto.For ?? string.Empty;
            existingJobRequestCar.Location = dto.Location ?? string.Empty;
            existingJobRequestCar.LocationTime = locationTime;
            existingJobRequestCar.Origin = dto.Origin ?? string.Empty;
            existingJobRequestCar.Destination = dto.Destination ?? string.Empty;
            existingJobRequestCar.StartDate = startDate;
            existingJobRequestCar.StartTime = startTime;
            existingJobRequestCar.EndDate = endDate;
            existingJobRequestCar.EndTime = endTime;
            existingJobRequestCar.NumPer = dto.NumPer ?? 0;
            existingJobRequestCar.Tel = dto.Tel ?? string.Empty;
            existingJobRequestCar.Note = dto.Note ?? string.Empty;
            existingJobRequestCar.PerApplicant = dto.PerApplicant ?? string.Empty;
            existingJobRequestCar.PerPosition = dto.PerPosition ?? string.Empty;
            // existingJobRequestCar.JobStatusId = jobStatusId;
            // existingJobRequestCar.ImageEmpId = dto.ImageEmpId;
            // existingJobRequestCar.GarageId = dto.GarageId;
        }
        catch (FormatException ex)
        {
            throw new ArgumentException($"Invalid date/time format: {ex.Message}");
        }

        _context.JobRequestCars.Update(existingJobRequestCar);
        await _context.SaveChangesAsync();

        // โหลดข้อมูลที่เกี่ยวข้องใหม่
        await _context.Entry(existingJobRequestCar)
            .Collection(j => j.ImageFiles)
            .LoadAsync();

        return existingJobRequestCar;
    }

    // Helper methods สำหรับแปลงวันที่และเวลา
    private DateOnly ParseDate(string? dateString)
    {
        if (string.IsNullOrEmpty(dateString))
            throw new ArgumentException("Date string is null or empty");

        // ลอง parse ด้วยหลาย format
        string[] dateFormats = new[]
        {
            "yyyy-MM-dd",
            "dd/MM/yyyy",
            "MM/dd/yyyy",
            "yyyy/MM/dd",
            "dd-MM-yyyy"
        };

        if (DateOnly.TryParseExact(dateString, dateFormats,
            CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            return date;
        }

        // ถ้าไม่ได้ format ข้างต้น ลอง parse แบบปกติ
        if (DateOnly.TryParse(dateString, out date))
        {
            return date;
        }

        throw new FormatException($"Cannot parse date: {dateString}. Expected formats: yyyy-MM-dd, dd/MM/yyyy, MM/dd/yyyy");
    }

    private TimeOnly ParseTime(string? timeString)
    {
        if (string.IsNullOrEmpty(timeString))
            throw new ArgumentException("Time string is null or empty");

        // ลอง parse ด้วยหลาย format
        string[] timeFormats = new[]
        {
            "HH:mm",
            "HH:mm:ss",
            "h:mm tt",
            "hh:mm tt",
            "H:mm"
        };

        if (TimeOnly.TryParseExact(timeString, timeFormats,
            CultureInfo.InvariantCulture, DateTimeStyles.None, out var time))
        {
            return time;
        }

        // ถ้าไม่ได้ format ข้างต้น ลอง parse แบบปกติ
        if (TimeOnly.TryParse(timeString, out time))
        {
            return time;
        }

        throw new FormatException($"Cannot parse time: {timeString}. Expected formats: HH:mm, HH:mm:ss, h:mm tt");
    }

    public async Task<JobRequestCar?> UpdateJobRequestCarPartialAsync(int id, JobRequestCarPartialUpdateDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto), "Input data is null");

        // Find existing job request car
        var existingJobRequestCar = await _context.JobRequestCars
            .Include(j => j.ImageFiles)
            .Include(j => j.JobStatus)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (existingJobRequestCar == null)
            return null;

        bool hasChanges = false;

        // Update EDateNow if provided
        if (dto.StatusDate.HasValue)
        {
            existingJobRequestCar.StatusDate = dto.StatusDate.Value;
            hasChanges = true;
        }

        // Update ETimeNow if provided
        if (dto.StatusTime.HasValue)
        {
            existingJobRequestCar.StatusTime = dto.StatusTime.Value;
            hasChanges = true;
        }

        // Update JobStatusId if provided
        if (dto.JobStatusId.HasValue) // เปลี่ยนจาก string.IsNullOrEmpty เป็น .HasValue
        {
            var existingStatus = await _context.JobStatuses
                .FindAsync(dto.JobStatusId.Value); // ใช้ .Value

            if (existingStatus == null)
            {
                throw new ArgumentException($"JobStatusId '{dto.JobStatusId}' not found in database");
            }

            existingJobRequestCar.JobStatusId = dto.JobStatusId.Value; // ใช้ .Value
            hasChanges = true;
        }

        // Save changes only if there are updates
        if (hasChanges)
        {
            await _context.SaveChangesAsync();
        }

        return existingJobRequestCar;
    }

    public async Task<JobRequestCar?> UpdateJobDistributeCarPartialAsync(int id, JobDistributeCarPartialDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto), "Input data is null");

        // Find existing job request car
        var existingJobRequestCar = await _context.JobRequestCars
            .Include(j => j.ImageFiles)
            .Include(j => j.JobStatus)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (existingJobRequestCar == null)
            return null;

        bool hasChanges = false;

        // Update JDDate if provided
        if (dto.JDDate.HasValue)
        {
            existingJobRequestCar.JDDate = dto.JDDate.Value;
            hasChanges = true;
        }

        // Update JobNumber if provided
        if (!string.IsNullOrEmpty(dto.JobNumber)) // ใช้ string.IsNullOrEmpty แทน .HasValue
        {
            existingJobRequestCar.JobNumber = dto.JobNumber; // ไม่ต้องใช้ .Value
            hasChanges = true;
        }

        // Update JDTime if provided
        if (dto.JDTime.HasValue)
        {
            existingJobRequestCar.JDTime = dto.JDTime.Value;
            hasChanges = true;
        }

        // Update ImageEmpId if provided
        if (dto.ImageEmpId.HasValue) 
        {
            var existingDriver = await _context.ImageEmps
                .FindAsync(dto.ImageEmpId.Value);

            if (existingDriver == null)
            {
                throw new ArgumentException($"ImageEmpId '{dto.ImageEmpId}' not found in database");
            }

            existingJobRequestCar.ImageEmpId = dto.ImageEmpId.Value;
            hasChanges = true;
        }

        // Update GarageId if provided
        if (dto.GarageId.HasValue) 
        {
            var existingCar = await _context.Garages
                .FindAsync(dto.GarageId.Value);

            if (existingCar == null)
            {
                throw new ArgumentException($"GarageId '{dto.GarageId}' not found in database");
            }

            existingJobRequestCar.GarageId = dto.GarageId.Value;
            hasChanges = true;
        }

        // Update JobStatusId to 2 (รับงาน) เมื่อมีการจ่ายงาน
        if (hasChanges)
        {
            existingJobRequestCar.JobStatusId = 2; // 2 = รับงาน
            existingJobRequestCar.StatusDate = DateOnly.FromDateTime(DateTime.Now);
            existingJobRequestCar.StatusTime = TimeOnly.FromDateTime(DateTime.Now);
        }

        // Save changes only if there are updates
        if (hasChanges)
        {
            await _context.SaveChangesAsync();
        }

        return existingJobRequestCar;
    }

    public async Task<JobRequestCar?> UpdateJobAcceptingAsync(int id, JobAcceptingDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto), "Input data is null");

        // Find existing job request car
        var existingJobRequestCar = await _context.JobRequestCars
            .Include(j => j.ImageFiles)
            .Include(j => j.JobStatus)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (existingJobRequestCar == null)
            return null;

        bool hasChanges = false;



        // Update EDateNow if provided
        if (dto.IssueDate.HasValue)
        {
            existingJobRequestCar.IssueDate = dto.IssueDate.Value;
            hasChanges = true;
        }

        // Update ETimeNow if provided
        if (dto.IssueTime.HasValue)
        {
            existingJobRequestCar.IssueTime = dto.IssueTime.Value;
            hasChanges = true;
        }
        // Update EDateNow if provided
        if (dto.ReturnDate.HasValue)
        {
            existingJobRequestCar.ReturnDate = dto.ReturnDate.Value;
            hasChanges = true;
        }

        // Update ETimeNow if provided
        if (dto.ReturnTime.HasValue)
        {
            existingJobRequestCar.ReturnTime = dto.ReturnTime.Value;
            hasChanges = true;
        }
        if (dto.UpNDate.HasValue)
        {
            existingJobRequestCar.UpNDate = dto.UpNDate.Value;
            hasChanges = true;
        }

        // Update ETimeNow if provided
        if (dto.UpNTime.HasValue)
        {
            existingJobRequestCar.UpNTime = dto.UpNTime.Value;
            hasChanges = true;
        }
        // Update JobStatusId if provided
        if (dto.JobStatusId.HasValue) // เปลี่ยนจาก string.IsNullOrEmpty เป็น .HasValue
        {
            var existingStatus = await _context.JobStatuses
                .FindAsync(dto.JobStatusId.Value); // ใช้ .Value

            if (existingStatus == null)
            {
                throw new ArgumentException($"JobStatusId '{dto.JobStatusId}' not found in database");
            }

            existingJobRequestCar.JobStatusId = dto.JobStatusId.Value; // ใช้ .Value
            hasChanges = true;
        }

        // Update JobStatusId if provided
        try
        {
            existingJobRequestCar.NumOil = dto.NumOil ?? 0;
            existingJobRequestCar.MileageOut = dto.MileageOut ?? string.Empty;
            existingJobRequestCar.MileageBack = dto.MileageBack ?? string.Empty;
            existingJobRequestCar.Ot = dto.Ot ?? string.Empty;
            existingJobRequestCar.Price = dto.Price ?? string.Empty;
        }
        catch (FormatException ex)
        {
            throw new ArgumentException($"Invalid date/time format: {ex.Message}");
        }

        // Save changes only if there are updates
        if (hasChanges)
        {
            await _context.SaveChangesAsync();
        }

        return existingJobRequestCar;
    }

    public async Task<IEnumerable<JobStatus>> GetAllJobStatusesAsync()
    {
        return await _context.JobStatuses
            .AsNoTracking()
            .OrderBy(s => s.Id)
            .ToListAsync();
    }

    public async Task<JobStatus?> GetJobStatusByIdAsync(int id) // เปลี่ยนจาก string เป็น int
    {
        return await _context.JobStatuses
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<JobStatus> CreateJobStatusAsync(JobStatusDto dto)
    {
        // Check if status already exists
        var existingStatus = await _context.JobStatuses
            .FirstOrDefaultAsync(s => s.Id == dto.Id || s.Status == dto.Status);

        if (existingStatus != null)
        {
            throw new InvalidOperationException(
                $"Job status with ID '{dto.Id}' or Status '{dto.Status}' already exists");
        }

        var jobStatus = new JobStatus
        {
            Id = dto.Id, // ถ้า Id เป็น int ใน DTO ก็ต้องเป็น int เช่นกัน
            Status = dto.Status,
            Description = dto.Description
        };

        _context.JobStatuses.Add(jobStatus);
        await _context.SaveChangesAsync();

        return jobStatus;
    }

    public async Task<JobStatus?> UpdateJobStatusAsync(int id, JobStatusDto dto) // เปลี่ยนจาก string เป็น int
    {
        var jobStatus = await _context.JobStatuses
            .FirstOrDefaultAsync(s => s.Id == id);

        if (jobStatus == null)
            return null;

        // Check if new status name conflicts with others (excluding current)
        if (!string.IsNullOrEmpty(dto.Status) && dto.Status != jobStatus.Status)
        {
            var duplicateStatus = await _context.JobStatuses
                .AnyAsync(s => s.Status == dto.Status && s.Id != id);

            if (duplicateStatus)
            {
                throw new InvalidOperationException(
                    $"Job status '{dto.Status}' already exists");
            }
        }

        // Update properties if provided
        if (!string.IsNullOrEmpty(dto.Status))
            jobStatus.Status = dto.Status;

        // if (dto.Description != null)
        //     jobStatus.Description = dto.Description;

        await _context.SaveChangesAsync();
        return jobStatus;
    }

    public async Task<bool> DeleteJobStatusAsync(int id) // เปลี่ยนจาก string เป็น int
    {
        // Check if status is being used by any job request
        var isInUse = await _context.JobRequestCars
            .AnyAsync(j => j.JobStatusId == id); // ใช้ int

        if (isInUse)
        {
            throw new InvalidOperationException(
                $"Cannot delete job status '{id}' because it is in use by job requests");
        }

        var jobStatus = await _context.JobStatuses
            .FirstOrDefaultAsync(s => s.Id == id); // ใช้ int

        if (jobStatus == null)
            return false;

        _context.JobStatuses.Remove(jobStatus);
        await _context.SaveChangesAsync();

        return true;
    }
    public async Task<JobRequestCar?> DistributeJobWithNumberAsync(int id, JobDistributeCarFullDto dto)
{
    try
    {
        // ค้นหางานจากฐานข้อมูล
        var jobRequest = await _context.JobRequestCars
            .Include(j => j.JobStatus)
            .Include(j => j.ImageEmp)
            .Include(j => j.Garage)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (jobRequest == null)
        {
            throw new ArgumentException($"ไม่พบงานหมายเลข {id}");
        }

        // ตรวจสอบสถานะ
        if (jobRequest.JobStatusId == 4) // 4 = ยกเลิก
        {
            throw new InvalidOperationException("ไม่สามารถจ่ายงานที่ถูกยกเลิกได้");
        }

        // ตรวจสอบคนขับ
        if (dto.ImageEmpId.HasValue && dto.ImageEmpId.Value > 0)
        {
            var driver = await _context.ImageEmps
                .FirstOrDefaultAsync(e => e.Id == dto.ImageEmpId.Value);
            
            if (driver == null)
            {
                throw new ArgumentException($"ไม่พบคนขับหมายเลข {dto.ImageEmpId}");
            }
        }

        // ตรวจสอบรถ
        if (dto.GarageId.HasValue && dto.GarageId.Value > 0)
        {
            var car = await _context.Garages
                .FirstOrDefaultAsync(g => g.Id == dto.GarageId.Value);
            
            if (car == null)
            {
                throw new ArgumentException($"ไม่พบรถหมายเลข {dto.GarageId}");
            }
        }

        // สร้างเลขรันถ้ายังไม่มี
        string jobNumber = dto.JobNumber;
        if (string.IsNullOrEmpty(jobNumber))
        {
            // สร้างเลขรันใหม่
            var prefix = DateTime.Now.ToString("yyMMdd");
            
            // ดึงตัวนับล่าสุด
            var today = DateOnly.FromDateTime(DateTime.Now);
            var latestJob = await _context.JobRequestCars
                .Where(j => j.JDDate == today && !string.IsNullOrEmpty(j.JobNumber))
                .OrderByDescending(j => j.JobNumber)
                .FirstOrDefaultAsync();
            
            int nextCounter = 1;
            if (latestJob != null && !string.IsNullOrEmpty(latestJob.JobNumber))
            {
                var numberPart = latestJob.JobNumber.Substring(Math.Max(0, latestJob.JobNumber.Length - 6));
                if (int.TryParse(numberPart, out int lastCounter))
                {
                    nextCounter = lastCounter + 1;
                }
            }
            
            jobNumber = $"{prefix}{nextCounter.ToString("D6")}";
        }

        // อัพเดทข้อมูล
        jobRequest.ImageEmpId = dto.ImageEmpId;
        jobRequest.GarageId = dto.GarageId;
        jobRequest.JDDate = dto.JDDate ?? DateOnly.FromDateTime(DateTime.Now);
        jobRequest.JDTime = dto.JDTime ?? TimeOnly.FromDateTime(DateTime.Now);
        jobRequest.JobStatusId = dto.JobStatusId;
        jobRequest.JobNumber = jobNumber;
        jobRequest.StatusDate = DateOnly.FromDateTime(DateTime.Now);
        jobRequest.StatusTime = TimeOnly.FromDateTime(DateTime.Now);

        await _context.SaveChangesAsync();

        // โหลดข้อมูลที่เกี่ยวข้อง
        await _context.Entry(jobRequest)
            .Reference(j => j.JobStatus)
            .LoadAsync();
        await _context.Entry(jobRequest)
            .Reference(j => j.ImageEmp)
            .LoadAsync();
        await _context.Entry(jobRequest)
            .Reference(j => j.Garage)
            .LoadAsync();

        Console.WriteLine($"✅ จ่ายงานสำเร็จ: #{jobNumber} สำหรับงาน {id}");
        
        return jobRequest;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error distributing job: {ex.Message}");
        throw;
    }
}

public async Task<JobRequestCar?> UpdateJobNumberAsync(int id, string jobNumber)
{
    try
    {
        var jobRequest = await _context.JobRequestCars
            .FirstOrDefaultAsync(j => j.Id == id);

        if (jobRequest == null)
            return null;

        jobRequest.JobNumber = jobNumber;
        await _context.SaveChangesAsync();

        return jobRequest;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error updating job number: {ex.Message}");
        return null;
    }
}

    public async Task<JobNumberResultDto> GetJobNumberInfoForJobAsync(int jobId)
    {
        var result = new JobNumberResultDto
        {
            JobId = jobId,
            Success = false
        };

        try
        {
            var job = await _context.JobRequestCars
                .FirstOrDefaultAsync(j => j.Id == jobId);

            if (job == null)
            {
                result.Message = $"ไม่พบงานหมายเลข {jobId}";
                return result;
            }

            if (string.IsNullOrEmpty(job.JobNumber))
            {
                result.Message = "งานนี้ยังไม่มีเลขรัน";
                return result;
            }

            result.Success = true;
            result.JobNumber = job.JobNumber;
            result.Message = "ดึงข้อมูลเลขรันสำเร็จ";
            result.GeneratedAt = DateTime.Now;

            return result;
        }
        catch (Exception ex)
        {
            result.Message = $"เกิดข้อผิดพลาด: {ex.Message}";
            return result;
        }
    }

    
}
    //  public async Task<JobRequestCar> CreateJobRequestCarAsync(JobRequestCarDto dto)
    // {
    //     if (dto == null)
    //         throw new ArgumentNullException(nameof(dto), "Input data is null");

    //     var jobRequestCar = new JobRequestCar
    //     {
    //         DateNow = dto.DateNow,
    //         TimeNow = dto.TimeNow,
    //         EDateNow = dto.EDateNow,
    //         ETimeNow = dto.ETimeNow,
    //         Requester = dto.Requester ?? string.Empty,
    //         DepartmentId = dto.DepartmentId ?? string.Empty,
    //         Origin = dto.Origin ?? string.Empty,
    //         Destination = dto.Destination ?? string.Empty,
    //         StartDate = dto.StartDate,
    //         StartTime = dto.StartTime,
    //         EndDate = dto.EndDate,
    //         EndTime = dto.EndTime,
    //         NumPer = dto.NumPer ?? 0,
    //         Tel = dto.Tel ?? string.Empty,
    //         Note = dto.Note ?? string.Empty,
    //         ImageFiles = dto.ImageFiles?.Select(img => new JobImage
    //         {
    //             FileName = img.FileName ?? string.Empty,
    //             ImageFile = img.ImageFile ?? Array.Empty<byte>()
    //         }).ToList() ?? new List<JobImage>()
    //     };

    //     _context.JobRequestCars.Add(jobRequestCar);
    //     await _context.SaveChangesAsync();

    //     // Reload with related data
    //     await _context.Entry(jobRequestCar)
    //         .Collection(j => j.ImageFiles)
    //         .LoadAsync();

    //     return jobRequestCar;
    // }
    
    // public async Task<JobRequestCar?> UpdateJobRequestCarAsync(int id, JobRequestCarDto dto)
    // {
    //     if (dto == null)
    //         throw new ArgumentNullException(nameof(dto), "Input data is null");

    //     // Find existing job request car
    //     var existingJobRequestCar = await _context.JobRequestCars
    //         .Include(j => j.ImageFiles)
    //         .FirstOrDefaultAsync(j => j.Id == id);

    //     if (existingJobRequestCar == null)
    //         return null;

    //     // Validate JobStatusId if provided
    //     string jobStatusId = existingJobRequestCar.JobStatusId;
        
    //     if (!string.IsNullOrEmpty(dto.JobStatusId))
    //     {
    //         var existingStatus = await _context.JobStatuses
    //             .FindAsync(dto.JobStatusId);

    //         if (existingStatus == null)
    //         {
    //             throw new ArgumentException($"JobStatusId '{dto.JobStatusId}' not found in database");
    //         }

    //         jobStatusId = dto.JobStatusId;
    //     }

    //     // Update properties
    //     existingJobRequestCar.DateNow = dto.DateNow;
    //     existingJobRequestCar.TimeNow = dto.TimeNow;
    //     existingJobRequestCar.EDateNow = dto.EDateNow;
    //     existingJobRequestCar.ETimeNow = dto.ETimeNow;
    //     existingJobRequestCar.Requester = dto.Requester ?? string.Empty;
    //     existingJobRequestCar.DepartmentId = dto.DepartmentId ?? string.Empty;
    //     existingJobRequestCar.Origin = dto.Origin ?? string.Empty;
    //     existingJobRequestCar.Destination = dto.Destination ?? string.Empty;
    //     existingJobRequestCar.StartDate = dto.StartDate;
    //     existingJobRequestCar.StartTime = dto.StartTime;
    //     existingJobRequestCar.EndDate = dto.EndDate;
    //     existingJobRequestCar.EndTime = dto.EndTime;
    //     existingJobRequestCar.NumPer = dto.NumPer ?? 0;
    //     existingJobRequestCar.Tel = dto.Tel ?? string.Empty;
    //     existingJobRequestCar.Note = dto.Note ?? string.Empty;
    //     existingJobRequestCar.JobStatusId = jobStatusId;

    //     // Handle image updates
    //     if (dto.ImageFiles != null)
    //     {
    //         // Remove existing images
    //         _context.JobImages.RemoveRange(existingJobRequestCar.ImageFiles);
            
    //         // Add new images
    //         existingJobRequestCar.ImageFiles = dto.ImageFiles.Select(img => new JobImage
    //         {
    //             FileName = img.FileName ?? string.Empty,
    //             ImageFile = img.ImageFile ?? Array.Empty<byte>()
    //         }).ToList();
    //     }

    //     await _context.SaveChangesAsync();

    //     // Reload with related data
    //     await _context.Entry(existingJobRequestCar)
    //         .Collection(j => j.ImageFiles)
    //         .LoadAsync();
    //     await _context.Entry(existingJobRequestCar)
    //         .Reference(j => j.JobStatus)
    //         .LoadAsync();

    //     return existingJobRequestCar;
    // }
    