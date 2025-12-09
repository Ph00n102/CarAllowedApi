using CarAllowedApi.Data;
using CarAllowedApi.Dto;
using Microsoft.EntityFrameworkCore;

namespace CarAllowedApi.Services;

public interface IJobRequestCarService
{
    Task<JobRequestCar[]> GetAllJobRequestCarsAsync();
    Task<JobRequestCar?> GetJobRequestCarByIdAsync(int id);
    Task<JobRequestCar> CreateJobRequestCarAsync(JobRequestCarDto dto);
    Task<JobRequestCar?> UpdateJobRequestCarAsync(int id, JobRequestCarDto dto);
    Task<JobRequestCar?> UpdateJobRequestCarPartialAsync(int id, JobRequestCarPartialUpdateDto dto);
    
    // เปลี่ยน string เป็น int
    Task<IEnumerable<JobStatus>> GetAllJobStatusesAsync();
    Task<JobStatus?> GetJobStatusByIdAsync(int id); // เปลี่ยนจาก string เป็น int
    Task<JobStatus> CreateJobStatusAsync(JobStatusDto dto);
    Task<JobStatus?> UpdateJobStatusAsync(int id, JobStatusDto dto); // เปลี่ยนจาก string เป็น int
    Task<bool> DeleteJobStatusAsync(int id); // เปลี่ยนจาก string เป็น int
}

public class JobRequestCarService : IJobRequestCarService
{
    private readonly ApplicationDbContext _context;

    public JobRequestCarService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<JobRequestCar[]> GetAllJobRequestCarsAsync()
    {
        return await _context.JobRequestCars
            .Include(j => j.ImageFiles)
            .Include(j => j.JobStatus) // เพิ่ม Include JobStatus
            .AsNoTracking()
            .ToArrayAsync();
    }

    public async Task<JobRequestCar?> GetJobRequestCarByIdAsync(int id)
    {
        return await _context.JobRequestCars
            .Include(j => j.ImageFiles)
            .Include(j => j.JobStatus) // เพิ่ม Include JobStatus
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == id);
    }

    public async Task<JobRequestCar> CreateJobRequestCarAsync(JobRequestCarDto dto)
{
    if (dto == null)
        throw new ArgumentNullException(nameof(dto), "Input data is null");

    int jobStatusId;

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
                    FileName = imageFile.FileName ?? string.Empty,
                    ImageFile = imageBytes,
                    OpNoteDetailId = 0
                });
            }
        }
    }

    var jobRequestCar = new JobRequestCar
    {
        DateNow = dto.DateNow,
        TimeNow = dto.TimeNow,
        EDateNow = dto.EDateNow,
        ETimeNow = dto.ETimeNow,
        Requester = dto.Requester ?? string.Empty,
        DepartmentId = dto.DepartmentId ?? string.Empty,
        Origin = dto.Origin ?? string.Empty,
        Destination = dto.Destination ?? string.Empty,
        StartDate = dto.StartDate,
        StartTime = dto.StartTime,
        EndDate = dto.EndDate,
        EndTime = dto.EndTime,
        NumPer = dto.NumPer ?? 0,
        Tel = dto.Tel ?? string.Empty,
        Note = dto.Note ?? string.Empty,
        JobStatusId = jobStatusId,
        ImageEmpId = dto.ImageEmpId,
        GarageId = dto.GarageId,
        ImageFiles = jobImages
    };

    _context.JobRequestCars.Add(jobRequestCar);
    await _context.SaveChangesAsync();

    await _context.Entry(jobRequestCar)
        .Collection(j => j.ImageFiles)
        .LoadAsync();

    return jobRequestCar;
}

    public async Task<JobRequestCar?> UpdateJobRequestCarAsync(int id, JobRequestCarDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto), "Input data is null");

        // Find existing job request car
        var existingJobRequestCar = await _context.JobRequestCars
            .Include(j => j.ImageFiles)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (existingJobRequestCar == null)
            return null;

        // Validate JobStatusId if provided
        int jobStatusId = existingJobRequestCar.JobStatusId;
        
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

        // Update properties
        existingJobRequestCar.DateNow = dto.DateNow;
        existingJobRequestCar.TimeNow = dto.TimeNow;
        existingJobRequestCar.EDateNow = dto.EDateNow;
        existingJobRequestCar.ETimeNow = dto.ETimeNow;
        existingJobRequestCar.Requester = dto.Requester ?? string.Empty;
        existingJobRequestCar.DepartmentId = dto.DepartmentId ?? string.Empty;
        existingJobRequestCar.Origin = dto.Origin ?? string.Empty;
        existingJobRequestCar.Destination = dto.Destination ?? string.Empty;
        existingJobRequestCar.StartDate = dto.StartDate;
        existingJobRequestCar.StartTime = dto.StartTime;
        existingJobRequestCar.EndDate = dto.EndDate;
        existingJobRequestCar.EndTime = dto.EndTime;
        existingJobRequestCar.NumPer = dto.NumPer ?? 0;
        existingJobRequestCar.Tel = dto.Tel ?? string.Empty;
        existingJobRequestCar.Note = dto.Note ?? string.Empty;
        existingJobRequestCar.JobStatusId = jobStatusId;
        existingJobRequestCar.ImageEmpId = dto.ImageEmpId;
        existingJobRequestCar.GarageId = dto.GarageId;

        // Handle image updates
        if (dto.ImageFiles != null)
        {
            // Remove existing images
            if (existingJobRequestCar.ImageFiles != null)
            {
                _context.JobImages.RemoveRange(existingJobRequestCar.ImageFiles);
            }
            
            // Add new images
            // var jobImages = new List<JobImage>();
            // foreach (var imgDto in dto.ImageFiles)
            // {
            //     byte[] imageBytes = Array.Empty<byte>();
                
            //     if (imgDto.ImageFile != null && imgDto.ImageFile.Length > 0)
            //     {
            //         imageBytes = imgDto.ImageFile;
            //     }

            //     jobImages.Add(new JobImage
            //     {
            //         FileName = imgDto.FileName ?? string.Empty,
            //         ImageFile = imageBytes,
            //         OpNoteDetailId = 0
            //     });
            // }
            
            // existingJobRequestCar.ImageFiles = jobImages;
        }

        await _context.SaveChangesAsync();

        // Reload with related data
        await _context.Entry(existingJobRequestCar)
            .Collection(j => j.ImageFiles)
            .LoadAsync();
        
        await _context.Entry(existingJobRequestCar)
            .Reference(j => j.JobStatus)
            .LoadAsync();

        return existingJobRequestCar;
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
        if (dto.EDateNow.HasValue)
        {
            existingJobRequestCar.EDateNow = dto.EDateNow.Value;
            hasChanges = true;
        }

        // Update ETimeNow if provided
        if (dto.ETimeNow.HasValue)
        {
            existingJobRequestCar.ETimeNow = dto.ETimeNow.Value;
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
    