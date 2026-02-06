using CarAllowedApi.Data;
using CarAllowedApi.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CarAllowedApi.Services
{
    public interface IImageEmpService
    {
        Task<ImageEmp[]> GetImageAsync();
        Task<ImageEmp?> GetImageByIdFirstAsync(int id);
        Task<ImageEmp> SaveImageAsync(ImageEmpDto dto);
        Task<ImageEmp> UpdateImageAsync(ImageEmpUpdateDto dto);
        Task DeleteImageAsync(int id);

        Task<ImageGDto[]> GetCarImageAsync();
        Task<Garage?> GetCarImageByIdFirstAsync(int id);
        Task<Garage> SaveCarImageAsync(GarageDto dto);
        Task<Garage> UpdateCarImageAsync(GarageDto dto);
        Task DeleteCarImageAsync(int id);
    }

    public class ImageEmpService : IImageEmpService
    {
        private readonly ApplicationDbContext _dbContext;

        public ImageEmpService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ImageEmp[]> GetImageAsync()
        {
            return await _dbContext.ImageEmps
                .AsNoTracking()
                .OrderByDescending(x => x.Id) // เรียงตาม ID ล่าสุดขึ้นก่อน
                .ToArrayAsync();
        }

        public async Task<ImageEmp?> GetImageByIdFirstAsync(int id)
        {
            return await _dbContext.ImageEmps
                .AsNoTracking()
                .FirstOrDefaultAsync(img => img.Id == id);
        }

        public async Task<ImageEmp> SaveImageAsync(ImageEmpDto dto)
        {
            if (dto.ImageFile == null || dto.ImageFile.Length == 0)
                throw new InvalidOperationException("No image file was supplied.");

            // ตรวจสอบข้อมูลที่จำเป็น
            if (string.IsNullOrEmpty(dto.Name))
                throw new InvalidOperationException("Name is required.");
            if (string.IsNullOrEmpty(dto.Nickname))
                throw new InvalidOperationException("Nickname is required.");
            if (string.IsNullOrEmpty(dto.Tel))
                throw new InvalidOperationException("Tel is required.");
            if (string.IsNullOrEmpty(dto.EmpStatusId))
                throw new InvalidOperationException("Tel is required.");

            byte[] bytes;
            await using (var ms = new MemoryStream())
            {
                await dto.ImageFile.CopyToAsync(ms);
                bytes = ms.ToArray();
            }

            var entity = new ImageEmp
            {
                Name = dto.Name,
                Nickname = dto.Nickname,
                Tel = dto.Tel,
                EmpStatusId = dto.EmpStatusId,
                FileName = string.IsNullOrEmpty(dto.FileName)
                    ? dto.ImageFile.FileName
                    : dto.FileName,
                ImageFile = bytes,
                CreatedAt = DateTime.Now
            };

            await _dbContext.ImageEmps.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<ImageEmp> UpdateImageAsync(ImageEmpUpdateDto dto)
        {
            var existing = await _dbContext.ImageEmps.FindAsync(dto.Id)
                          ?? throw new InvalidOperationException("Image not found");

            // อัปเดตข้อมูลพื้นฐาน
            if (!string.IsNullOrEmpty(dto.Name))
                existing.Name = dto.Name;

            if (!string.IsNullOrEmpty(dto.Nickname))
                existing.Nickname = dto.Nickname;

            if (!string.IsNullOrEmpty(dto.Tel))
                existing.Tel = dto.Tel;

            if (!string.IsNullOrEmpty(dto.EmpStatusId))
                existing.EmpStatusId = dto.EmpStatusId;

            // อัปเดตรูปภาพถ้ามี
            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                await using var ms = new MemoryStream();
                await dto.ImageFile.CopyToAsync(ms);

                existing.ImageFile = ms.ToArray();
                existing.FileName = dto.ImageFile.FileName;
            }

            existing.UpdatedAt = DateTime.Now;
            await _dbContext.SaveChangesAsync();
            return existing;
        }

        public async Task DeleteImageAsync(int id)
        {
            var image = await _dbContext.ImageEmps.FindAsync(id);
            if (image == null)
                throw new InvalidOperationException("Image not found");

            _dbContext.ImageEmps.Remove(image);
            await _dbContext.SaveChangesAsync();
        }


        // public async Task<Garage[]> GetCarImageAsync()
        // {
        //     return await _dbContext.Garages
        //         .AsNoTracking()
        //         .OrderByDescending(x => x.Id) // เรียงตาม ID ล่าสุดขึ้นก่อน
        //         .ToArrayAsync();
        // }
        public async Task<ImageGDto[]> GetCarImageAsync()
        {
            return await _dbContext.Garages
                .AsNoTracking()
                .OrderByDescending(x => x.Id)
                .Select(g => new ImageGDto
                {
                    // Map แต่ละ property จาก Garage ไปยัง ImageGDto
                    Id = g.Id,
                    CarRegistration = g.CarRegistration, // ตัวอย่าง property
                    Carmodel = g.Carmodel, // ตัวอย่าง property
                    CarStatusId = g.CarStatusId, // ตัวอย่าง property
                    CarProvince = g.CarProvince, // ตัวอย่าง property
                    Cartype = g.Cartype
                })
                .ToArrayAsync();
        }
        public async Task<Garage?> GetCarImageByIdFirstAsync(int id)
        {
            return await _dbContext.Garages
                .AsNoTracking()
                .FirstOrDefaultAsync(img => img.Id == id);
        }
        public async Task<Garage> SaveCarImageAsync(GarageDto dto)
        {
            if (dto.ImageFile == null || dto.ImageFile.Length == 0)
                throw new InvalidOperationException("No image file was supplied.");

            // ตรวจสอบข้อมูลที่จำเป็น
            if (string.IsNullOrEmpty(dto.CarRegistration))
                throw new InvalidOperationException("CarRegistration is required.");
            if (string.IsNullOrEmpty(dto.Carmodel))
                throw new InvalidOperationException("Carmodel is required.");
            if (string.IsNullOrEmpty(dto.Cartype))
                throw new InvalidOperationException("Tel is required.");

            byte[] bytes;
            await using (var ms = new MemoryStream())
            {
                await dto.ImageFile.CopyToAsync(ms);
                bytes = ms.ToArray();
            }

            var entity = new Garage
            {
                CarRegistration = dto.CarRegistration,
                Carmodel = dto.Carmodel,
                Cartype = dto.Cartype,
                CarStatusId = dto.CarStatusId,
                CarProvince = dto.CarProvince,
                FileName = string.IsNullOrEmpty(dto.FileName)
                    ? dto.ImageFile.FileName
                    : dto.FileName,
                ImageFile = bytes,
                CreatedAt = DateTime.Now
            };

            await _dbContext.Garages.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<Garage> UpdateCarImageAsync(GarageDto dto)
        {
            var existing = await _dbContext.Garages.FindAsync(dto.Id)
                          ?? throw new InvalidOperationException("Image not found");

            // อัปเดตข้อมูลพื้นฐาน
            if (!string.IsNullOrEmpty(dto.CarRegistration))
                existing.CarRegistration = dto.CarRegistration;
            if (!string.IsNullOrEmpty(dto.Carmodel))
                existing.Carmodel = dto.Carmodel;
            if (!string.IsNullOrEmpty(dto.Cartype))
                existing.Cartype = dto.Cartype;
            if (!string.IsNullOrEmpty(dto.CarStatusId))
                existing.CarStatusId = dto.CarStatusId;
            if (!string.IsNullOrEmpty(dto.CarProvince))
                existing.CarProvince = dto.CarProvince;

            // อัปเดตรูปภาพถ้ามี
            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                await using var ms = new MemoryStream();
                await dto.ImageFile.CopyToAsync(ms);

                existing.ImageFile = ms.ToArray();
                existing.FileName = dto.ImageFile.FileName;
            }

            existing.UpdatedAt = DateTime.Now;
            await _dbContext.SaveChangesAsync();
            return existing;
        }
        public async Task DeleteCarImageAsync(int id)
        {
            var image = await _dbContext.Garages.FindAsync(id);
            if (image == null)
                throw new InvalidOperationException("Image not found");

            _dbContext.Garages.Remove(image);
            await _dbContext.SaveChangesAsync();
        }
    }
}